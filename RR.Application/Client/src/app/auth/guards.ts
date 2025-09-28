import { inject } from '@angular/core';
import { CanActivateFn, Router, UrlTree, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from './auth.service';
import { Observable, map, catchError, of } from 'rxjs';

export const authGuard: CanActivateFn = (
  _route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot
): Observable<boolean | UrlTree> => {
  const auth = inject(AuthService);
  const router = inject(Router);

  // Se já tem dados do usuário carregados, usa verificação rápida
  if (auth.isLoggedIn()) {
    return of(true);
  }

  return auth.checkAuth().pipe(
    map((isAuthenticated:any)  =>
      isAuthenticated
        ? true
        : router.createUrlTree(['/auth/login'], { queryParams: { returnUrl: state.url } })
    ),
    catchError(() =>
      of(router.createUrlTree(['/auth/login'], { queryParams: { returnUrl: state.url } }))
    )
  );
};

export const noAuthGuard: CanActivateFn = (
  _route: ActivatedRouteSnapshot,
  _state: RouterStateSnapshot
): Observable<boolean | UrlTree> => {
  const auth = inject(AuthService);
  const router = inject(Router);

  if (auth.isLoggedIn()) {
    return of(router.createUrlTree(['/']));
  }

  // Caso contrário, verifica via API
  return auth.checkAuth().pipe(
    map(isAuthenticated =>
      !isAuthenticated
        ? true
        : router.createUrlTree(['/'])
    ),
    catchError(() =>
      of(true) // Se der erro na verificação, assume que não está logado
    )
  );
};
