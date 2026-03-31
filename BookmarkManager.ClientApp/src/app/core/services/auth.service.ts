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
   * Checks if the user is authenticated.
   */
  isAuthenticated(): boolean {
    const token = this.storage.getString(this.TOKEN_KEY);
    return token !== null && token.length > 0;
  }

  /**
   * Gets the JWT token for API calls.
   */
  getToken(): string | null {
    return this.storage.getString(this.TOKEN_KEY);
  }

  /**
   * Gets the current user's ID. Returns 0 if not authenticated.
   */
  getUserId(): number {
    const user = this.getStoredUser();
    return user?.userId ?? 0;
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