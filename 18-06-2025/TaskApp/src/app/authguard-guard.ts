import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

export const authguardGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);

  const isAuthenticated = !!localStorage.getItem('accessToken');

  if (isAuthenticated) {
    return true;
  } else {
    console.error('Access denied - User not authenticated');
    router.navigate(['/login']);
    return false;
  }
};
