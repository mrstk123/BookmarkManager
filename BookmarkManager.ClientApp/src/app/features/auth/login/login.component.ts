import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ApiService } from '../../../core/services/api.service';
import { AuthService } from '../../../core/services/auth.service';
import { AuthResponse } from '../../../models';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login-component',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent {
  private formBuilder = inject(FormBuilder);
  private api = inject(ApiService);
  private authService = inject(AuthService);
  private router = inject(Router);

  loginForm = this.formBuilder.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
  });

  showPassword = false;
  loginError = '';

  onSubmit(): void {
    if (this.loginForm.valid) {
      this.loginError = '';
      const credentials = this.loginForm.value as { email: string; password: string };
      this.api.login(credentials).subscribe({
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
          const errorMessage = (err as { error?: { message?: string } })?.error?.message;
          this.loginError = errorMessage || 'Login failed. Please try again.';
        },
      });
    }
  }

  togglePassword(): void {
    this.showPassword = !this.showPassword;
  }
}