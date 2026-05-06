import { Injectable, inject } from '@angular/core';
import { Router, CanActivateFn, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../../services/auth.service';

export const authGuard: CanActivateFn = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isAuthenticated()) {
    const requiredRole = route.data['role'];
    if (requiredRole && authService.getCurrentUser()?.role !== requiredRole) {
      router.navigate(['/dashboard']);
      return false;
    }
    return true;
  }
  router.navigate(['/login']);
  return false;
};
