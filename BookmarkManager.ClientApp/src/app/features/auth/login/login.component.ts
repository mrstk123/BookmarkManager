import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ApiService } from '../../../core/services/api.service';
import { AuthService } from '../../../core/services/auth.service';
import { LoginResponse } from '../../../models/login-response.model';
import { Router } from '@angular/router';

@Component({
    selector: 'app-login-component',
    imports: [ReactiveFormsModule, CommonModule, RouterModule],
    templateUrl: './login.component.html',
    styleUrl: './login.component.scss',
})
export class LoginComponent {

    private formBuilder = inject(FormBuilder);
    private api = inject(ApiService);
    private authService = inject(AuthService);
    private router = inject(Router);

    loginFrom = this.formBuilder.group({
        email: ['', [Validators.required, Validators.email]],
        password: ['', [Validators.required, Validators.minLength(6)]],
    });

    showPassword: boolean = false;

    onSubmit() {
        if (this.loginFrom.valid) {
            const credentials = this.loginFrom.value as { email: string; password: string };
            this.api.login(credentials).subscribe({
                next: (res: LoginResponse) => {
                    if (res.token) {
                        this.authService.login(res.token, res.id, res.fullName, res.email, res.userName);
                        this.router.navigate(['/bookmarks']);
                    }
                },
                error: (err: any) => {
                    console.error('Login API error', err);
                }
            });
        }
    }

    togglePassword(): void {
        this.showPassword = !this.showPassword;
    }

}