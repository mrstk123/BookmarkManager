import { Injectable, inject } from '@angular/core';
import { StorageService } from './storage.service';
import { StoredUser } from '../../models/stored-user.model';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly TOKEN_KEY = 'auth_token';
  private readonly USER_KEY = 'auth_user';

  private storage = inject(StorageService);

  /**
   * Stores authentication data after successful login.
   */
  login(token: string, user: StoredUser): void {
    this.storage.setString(this.TOKEN_KEY, token);
    this.storage.set(this.USER_KEY, user);
  }

  /**
   * Removes authentication data.
   */
  logout(): void {
    this.storage.remove(this.TOKEN_KEY);
    this.storage.remove(this.USER_KEY);
  }

  /**
   * Checks if the user is authenticated by verifying token existence and expiration.
   */
  isAuthenticated(): boolean {
    const token = this.storage.getString(this.TOKEN_KEY);
    if (!token || token.length === 0) return false;

    // Check if token is expired
    if (this.isTokenExpired(token)) {
      this.logout(); // Auto-logout if token is expired
      return false;
    }

    return true;
  }

  /**
   * Checks if a JWT token is expired by parsing the exp claim.
   */
  private isTokenExpired(token: string): boolean {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const exp = payload.exp;
      if (!exp) return true; // No expiration claim, treat as expired
      return Date.now() >= exp * 1000; // exp is in seconds
    } catch {
      return true; // Invalid token format, treat as expired
    }
  }

  /**
   * Gets the JWT token for API calls.
   */
  getToken(): string | null {
    return this.storage.getString(this.TOKEN_KEY);
  }

  /**
   * Gets the current user's ID. Returns null if not authenticated.
   */
  getUserId(): number | null {
    const user = this.getStoredUser();
    return user?.userId ?? null;
  }

  /**
   * Gets the stored user data.
   */
  getStoredUser(): StoredUser | null {
    return this.storage.get<StoredUser>(this.USER_KEY);
  }

  /**
   * Updates the stored user's full name (e.g., after profile update).
   */
  updateStoredFullName(fullName: string): void {
    const user = this.getStoredUser();
    if (user) {
      user.fullName = fullName;
      this.storage.set(this.USER_KEY, user);
    }
  }
}