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
    styleUrls: ['./forgot-password.component.scss'],
})
export class ForgotPasswordComponent {
    private formBuilder = inject(FormBuilder);
    private api = inject(ApiService);
    private router = inject(Router);

    forgotForm = this.formBuilder.group({
        email: ['', [Validators.required, Validators.email]],
    });

    forgotError = '';

    onSubmit() {
        if (this.forgotForm.valid) {
            this.forgotError = '';
            const data = this.forgotForm.value as { email: string };
            this.api.forgotPassword(data).subscribe({
                next: (res: ForgotPasswordResponse) => {
                    if (res && res.success) {
                        this.router.navigate(['/login']);
                    } else {
                        this.forgotError = res?.message || 'Request failed. Please try again.';
                    }
                },
                error: (err: any) => {
                    this.forgotError = err.error?.message || 'Request failed. Please try again.';
                }
            });
        }
    }
}