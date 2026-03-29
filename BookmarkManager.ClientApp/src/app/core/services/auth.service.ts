import { Injectable } from '@angular/core';

export interface StoredUser {
    userId: number;
    fullName: string;
    email: string;
    userName: string;
}

@Injectable({
    providedIn: 'root',
})
export class AuthService {
    private readonly TOKEN_KEY = 'auth_token';
    private readonly USER_KEY = 'auth_user';

    constructor() { }

    login(token: string, userId: number, fullName: string, email: string, userName: string) {
        localStorage.setItem(this.TOKEN_KEY, token);
        localStorage.setItem(this.USER_KEY, JSON.stringify({ userId, fullName, email, userName }));
    }

    logout() {
        localStorage.removeItem(this.TOKEN_KEY);
        localStorage.removeItem(this.USER_KEY);
    }

    isAuthenticated(): boolean {
        return !!localStorage.getItem(this.TOKEN_KEY);
    }

    getToken(): string | null {
        return localStorage.getItem(this.TOKEN_KEY);
    }

    getUserId(): number {
        const user = this.getStoredUser();
        return user?.userId ?? 0;
    }

    getStoredUser(): StoredUser | null {
        const raw = localStorage.getItem(this.USER_KEY);
        return raw ? JSON.parse(raw) : null;
    }

    updateStoredFullName(fullName: string) {
        const user = this.getStoredUser();
        if (user) {
            user.fullName = fullName;
            localStorage.setItem(this.USER_KEY, JSON.stringify(user));
        }
    }
}