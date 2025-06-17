import { Injectable } from '@angular/core';
import { CanActivate, CanActivateFn, Router } from '@angular/router';

@Injectable()
export class AuthGuard implements CanActivate {
  constructor(private router: Router) { }

  canActivate(): boolean {
    const isAuthenticated = !!localStorage.getItem('token'); 
    if (!isAuthenticated) {
      console.warn('Access denied - User not authenticated');
      this.router.navigate(['login']);
      return false;
    }
    return true;
  }
}