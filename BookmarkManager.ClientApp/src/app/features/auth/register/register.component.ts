import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../../core/services/api.service';
import { AuthService } from '../../../core/services/auth.service';
import { Router } from '@angular/router';
import { RegisterResponse } from '../../../models/register-response.model';
import { FormBuilder, ReactiveFormsModule, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { debounceTime, distinctUntilChanged, of, switchMap, map, catchError, first } from 'rxjs';

@Component({
    selector: 'app-register-component',
    standalone: true,
    imports: [ReactiveFormsModule, CommonModule, RouterModule],
    templateUrl: './register.component.html',
    styleUrl: './register.component.scss',
})
export class RegisterComponent {

    private formBuilder = inject(FormBuilder);
    private api = inject(ApiService);
    private authService = inject(AuthService);
    private router = inject(Router);

    registerFrom = this.formBuilder.group({
        email: ['', [Validators.required, Validators.email], [this.emailTakenValidator.bind(this)]],
        fullName: ['', [Validators.required]],
        password: ['', [Validators.required, Validators.minLength(6)]],
    });

    showPassword: boolean = false;

    togglePassword(): void {
        this.showPassword = !this.showPassword;
    }

    private emailTakenValidator(control: AbstractControl) {
        if (!control.value || control.hasError('required') || control.hasError('email')) {
            return of(null);
        }
        return of(control.value).pipe(
            debounceTime(500),
            distinctUntilChanged(),
            switchMap(email =>
                this.api.checkEmail(email).pipe(
                    map(exists => exists ? { emailTaken: true } : null),
                    catchError(() => of(null)),
                )
            ),
            first(),
        );
    }

    get emailErrors(): string | null {
        const emailCtrl = this.registerFrom.get('email');
        if (!emailCtrl || !emailCtrl.touched || !emailCtrl.errors) return null;
        if (emailCtrl.hasError('required')) return 'Email is required.';
        if (emailCtrl.hasError('email')) return 'Please enter a valid email.';
        if (emailCtrl.hasError('emailTaken')) return 'This email is already registered.';
        return null;
    }

    onSubmit() {
        if (this.registerFrom.valid) {
            var data = this.registerFrom.value as {
                userName: string;
                email: string;
                fullName: string;
                password: string
            };
            data.userName = data.email;
            this.api.register(data).subscribe({
                next: (res: RegisterResponse) => {
                    if (res.token) {
                        this.authService.login(res.token, res.id, res.fullName, res.email, res.userName);
                        this.router.navigate(['/bookmarks']);
                    }
                },
                error: (err: any) => {
                    console.error('Registration API error', err);
                }
            });
        }
    }
}