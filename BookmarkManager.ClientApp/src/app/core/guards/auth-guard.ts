import { inject } from '@angular/core';
import { Router, type CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isAuthenticated()) {
    const userId = authService.getUserId();
    if (userId !== null && userId > 0) {
      return true;
    }
  }

  return router.createUrlTree(['/login']);
};