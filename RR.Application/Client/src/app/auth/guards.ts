import { inject } from '@angular/core';
import { CanActivateFn, Router, UrlTree, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from './auth.service';

export const authGuard: CanActivateFn = (_route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree => {
  const auth = inject(AuthService);
  const router = inject(Router);
  return auth.isLoggedIn()
    ? true
    : router.createUrlTree(['/auth/login'], { queryParams: { returnUrl: state.url } });
};

export const noAuthGuard: CanActivateFn = (_route: ActivatedRouteSnapshot, _state: RouterStateSnapshot): boolean | UrlTree => {
  const auth = inject(AuthService);
  const router = inject(Router);
  return !auth.isLoggedIn()
    ? true
    : router.createUrlTree(['/']);
};
