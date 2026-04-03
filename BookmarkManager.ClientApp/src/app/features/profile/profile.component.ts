import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, FormsModule, Validators } from '@angular/forms';
import { ApiService } from '../../core/services/api.service';
import { UserProfile } from '../../models/user-profile.model';
import { AuthService } from '../../core/services/auth.service';

@Component({
    selector: 'app-profile',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, FormsModule],
    templateUrl: './profile.component.html',
    styleUrls: ['./profile.component.scss'],
})
export class ProfileComponent implements OnInit {
    private api = inject(ApiService);
    private authService = inject(AuthService);
    private fb = inject(FormBuilder);

    profile: UserProfile | null = null;

    isEditingName = false;
    editName = '';
    profileMessage = '';
    profileError = '';

    showPasswordModal = false;
    passwordForm = this.fb.group({
        currentPassword: ['', [Validators.required, Validators.minLength(6)]],
        newPassword: ['', [Validators.required, Validators.minLength(6)]],
        confirmPassword: ['', Validators.required],
    });
    passwordMessage = '';
    passwordError = '';
    showCurrentPassword = false;
    showNewPassword = false;
    showConfirmPassword = false;

    ngOnInit(): void {
        const stored = this.authService.getStoredUser();
        if (stored) {
            this.profile = {
                id: stored.userId,
                userName: stored.userName,
                fullName: stored.fullName,
                email: stored.email,
                createdAt: '',
            };
        }

        this.api.getProfile().subscribe({
            next: (profile) => {
                this.profile = profile;
            },
        });
    }

    startEditName(): void {
        this.editName = this.profile?.fullName || '';
        this.isEditingName = true;
        this.profileMessage = '';
        this.profileError = '';
    }

    cancelEditName(): void {
        this.isEditingName = false;
        this.editName = '';
    }

    onSaveName(): void {
        const name = this.editName.trim();
        if (!name) return;

        this.profileMessage = '';
        this.profileError = '';
        this.api.updateProfile({ fullName: name }).subscribe({
            next: (updated) => {
                this.profile = updated;
                this.authService.updateStoredFullName(updated.fullName);
                this.isEditingName = false;
                this.profileMessage = 'Profile updated successfully.';
            },
            error: (err) => {
                this.profileError = err.error?.message || 'Failed to update profile.';
            },
        });
    }

    openPasswordModal(): void {
        this.showPasswordModal = true;
        this.passwordForm.reset();
        this.passwordMessage = '';
        this.passwordError = '';
        this.showCurrentPassword = false;
        this.showNewPassword = false;
        this.showConfirmPassword = false;
    }

    closePasswordModal(): void {
        this.showPasswordModal = false;
    }

    onChangePassword(): void {
        if (this.passwordForm.invalid) return;
        this.passwordMessage = '';
        this.passwordError = '';

        const { currentPassword, newPassword, confirmPassword } = this.passwordForm.value;
        if (newPassword !== confirmPassword) {
            this.passwordError = 'New password and confirmation do not match.';
            return;
        }

        this.api.changePassword({ currentPassword: currentPassword!, newPassword: newPassword! }).subscribe({
            next: () => {
                this.passwordMessage = 'Password changed successfully.';
                this.passwordForm.reset();
            },
            error: (err) => {
                this.passwordError = err.error?.message || 'Failed to change password.';
            },
        });
    }

    toggleField(field: 'current' | 'new' | 'confirm'): void {
        if (field === 'current') this.showCurrentPassword = !this.showCurrentPassword;
        if (field === 'new') this.showNewPassword = !this.showNewPassword;
        if (field === 'confirm') this.showConfirmPassword = !this.showConfirmPassword;
    }
}
