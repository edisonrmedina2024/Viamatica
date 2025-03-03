import { ActivatedRouteSnapshot, Router, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';

export function authGuard(
  router: Router
): (
  next: ActivatedRouteSnapshot, 
  state: RouterStateSnapshot
) => Observable<boolean> | Promise<boolean> | boolean {
  
  return (next: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
    const token = JSON.parse(localStorage.getItem('token') || '{}');
    
    if (token) {
      return true;
    } else {
      router.navigate(['/']);
      return false;
    }
    
  };
}