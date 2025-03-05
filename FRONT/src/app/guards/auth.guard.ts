import { inject } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivateFn,
  Router,
  RouterStateSnapshot
} from '@angular/router';

export const authGuard: CanActivateFn = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot
) => {
  const router = inject(Router);

  // Enhanced logging function
  const logAuthDecision = (allowed: boolean, reason: string) => {
    console.group('🔐 Auth Guard Debug');
    console.log('Route:', state.url);
    console.log('Access Allowed:', allowed);
    console.log('Reason:', reason);
    console.trace(); // Provides stack trace for deeper debugging
    console.groupEnd();
  };

  try {
    // Retrieve token directly from localStorage
    const tokenString = localStorage.getItem('token');
    
    // Comprehensive token validation
    if (!tokenString) {
      logAuthDecision(false, 'No token found in localStorage');
      router.navigate(['/login']);
      return false;
    }

    // Parse token safely
    let token;
    try {
      token = JSON.parse(tokenString);
    } catch (parseError) {
      logAuthDecision(false, 'Invalid token format');
      router.navigate(['/login']);
      return false;
    }
    // Route permission check
    const allowedRoutes = JSON.parse(
      localStorage.getItem('menuOptions') || '[]'
    );
    
    const currentRoute = state.url.split('/').pop() || '';

    if (allowedRoutes.includes(currentRoute)) {
      logAuthDecision(true, 'Route access granted');
      return true;
    } else {
      logAuthDecision(false, 'Route not in allowed routes');
      router.navigate(['/unauthorized']);
      return false;
    }

  } catch (error) {
    console.error('🚨 Unexpected Auth Guard Error:', error);
    router.navigate(['/error']);
    return false;
  }
};