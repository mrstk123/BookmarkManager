import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Router } from '@angular/router';
import { Component, inject } from '@angular/core';
import { ApiService } from '../../../core/services/api.service';
import { ForgotPasswordResponse } from '../../../models/forgot-password-response.model';

@Component({
    selector: 'app-forgot-password-component',
    standalone: true,
    imports: [ReactiveFormsModule, CommonModule, RouterModule],
    templateUrl: './forgot-password.component.html',
    styleUrl: './forgot-password.component.scss',
})
export class ForgotPasswordComponent {
    private formBuilder = inject(FormBuilder);
    private api = inject(ApiService);
    private router = inject(Router);

    forgotFrom = this.formBuilder.group({
        email: ['', [Validators.required, Validators.email]],
    });

    onSubmit() {
        if (this.forgotFrom.valid) {
            const data = this.forgotFrom.value as { email: string };
            this.api.forgotPassword(data).subscribe({
                next: (res: ForgotPasswordResponse) => {
                    if (res && res.success) {
                        this.router.navigate(['/login']);
                    } else {
                        console.error('Forgot password failed', res?.message);
                    }
                },
                error: (err: any) => {
                    console.error('Forgot password API error', err);
                }
            });
        }
    }
}