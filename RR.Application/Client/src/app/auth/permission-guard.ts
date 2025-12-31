import { inject } from '@angular/core';
import { CanActivateFn, Router, UrlTree } from '@angular/router';
import { Observable, of, map, catchError } from 'rxjs';
import { AuthService } from './auth.service';
import { PermissionService } from '../domain/shared/services/permission.service';
import { EAccountPermission } from '../domain/enum/EAccountPermission';

export function minPermissionGuard(min: EAccountPermission): CanActivateFn {
  return (_route, state): Observable<boolean | UrlTree> => {
    const auth = inject(AuthService);
    const perm = inject(PermissionService);
    const router = inject(Router);

    // se já tem account carregado, decide rápido
    if (auth.isLoggedIn()) {
      return of(perm.hasMinPermission(min)
        ? true
        : router.createUrlTree(['/'], { queryParams: { returnUrl: state.url } })
      );
    }

    return auth.checkAuth().pipe(
      map(isOk => {
        if (!isOk) return router.createUrlTree(['/auth/login'], { queryParams: { returnUrl: state.url } });

        return perm.hasMinPermission(min)
          ? true
          : router.createUrlTree(['/'], { queryParams: { returnUrl: state.url } });
      }),
      catchError(() =>
        of(router.createUrlTree(['/auth/login'], { queryParams: { returnUrl: state.url } }))
      )
    );
  };
}

// atalho pra não ficar repetindo enum toda hora
export const adminOnlyGuard = minPermissionGuard(EAccountPermission.Admin);
export const managerOrAboveGuard = minPermissionGuard(EAccountPermission.Manager);
export const servantOrAboveGuard = minPermissionGuard(EAccountPermission.Servant);

