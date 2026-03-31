import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = (_route, _state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  try {
    if (authService.isAuthenticated()) {
      // Additional check: ensure user ID is valid
      if (authService.getUserId() > 0) {
        return true;
      }
    }
  } catch {
    // If any error occurs during auth check, clear state and redirect to login
    authService.logout();
  }

  router.navigate(['/login']);
  return false;
};