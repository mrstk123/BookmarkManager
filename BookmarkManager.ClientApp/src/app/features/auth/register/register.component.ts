import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../../core/services/api.service';
import { AuthService } from '../../../core/services/auth.service';
import { Router } from '@angular/router';
import { AuthResponse } from '../../../models';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators,
  AbstractControl,
  AsyncValidatorFn,
} from '@angular/forms';
import { RouterModule } from '@angular/router';
import { debounceTime, distinctUntilChanged, of, switchMap, map, catchError, first } from 'rxjs';

@Component({
  selector: 'app-register-component',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
})
export class RegisterComponent {
  private formBuilder = inject(FormBuilder);
  private api = inject(ApiService);
  private authService = inject(AuthService);
  private router = inject(Router);

  registerForm = this.formBuilder.group({
    email: [
      '',
      [Validators.required, Validators.email],
      [this.emailTakenValidator.bind(this)],
    ],
    fullName: ['', [Validators.required]],
    password: ['', [Validators.required, Validators.minLength(6)]],
  });

  showPassword = false;
  registerError = '';

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
      switchMap((email: string) =>
        this.api.checkEmail(email).pipe(
          map((exists) => (exists ? { emailTaken: true } : null)),
          catchError(() => of(null))
        )
      ),
      first()
    );
  }

  get emailErrors(): string | null {
    const emailCtrl = this.registerForm.get('email');
    if (!emailCtrl || !emailCtrl.touched || !emailCtrl.errors) return null;
    if (emailCtrl.hasError('required')) return 'Email is required.';
    if (emailCtrl.hasError('email')) return 'Please enter a valid email.';
    if (emailCtrl.hasError('emailTaken'))
      return 'This email is already registered.';
    return null;
  }

  onSubmit(): void {
    if (this.registerForm.valid) {
      this.registerError = '';
      const formValue = this.registerForm.value as {
        email: string;
        fullName: string;
        password: string;
      };
      const registrationData = {
        email: formValue.email,
        fullName: formValue.fullName,
        password: formValue.password,
        userName: '', // Backend will extract email prefix as username
      };
      this.api.register(registrationData).subscribe({
        next: (res: AuthResponse) => {
          this.authService.login(res.token, {
            userId: res.id,
            fullName: res.fullName,
            email: res.email,
            userName: res.userName,
          });
          this.router.navigate(['/bookmarks']);
        },
        error: (err: unknown) => {
          const errorMessage =
            (err as { error?: { message?: string } })?.error?.message;
          this.registerError = errorMessage || 'Registration failed. Please try again.';
        },
      });
    }
  }
}