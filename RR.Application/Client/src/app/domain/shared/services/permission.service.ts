import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { AuthService } from '../../../auth/auth.service';
import { EAccountPermission, getPermissionLevel } from '../../enum/EAccountPermission';

@Injectable({ providedIn: 'root' })
export class PermissionService {
  constructor(private auth: AuthService) {}

  // nível atual do usuário (0..3)
  getCurrentLevel(): number {
    const perm = (this.auth.Account as any)?.accountPermission ?? null;
    if (perm === null || perm === undefined) return 0;
    return getPermissionLevel(Number(perm));
  }

  hasMinPermission(min: EAccountPermission): boolean {
    const curr = this.getCurrentLevel();
    const req = getPermissionLevel(min);
    return curr >= req;
  }

  // helpers
  isAdminOrAbove(): boolean { return this.hasMinPermission(EAccountPermission.Admin); }
  isManagerOrAbove(): boolean { return this.hasMinPermission(EAccountPermission.Manager); }
  isServantOrAbove(): boolean { return this.hasMinPermission(EAccountPermission.Servant); }
  isStudentOrAbove(): boolean { return this.hasMinPermission(EAccountPermission.Student); }

  // versão observable (pra usar em template/pipe se quiser)
  hasMinPermission$(min: EAccountPermission): Observable<boolean> {
    return this.auth.currentUser$.pipe(
      map(() => this.hasMinPermission(min))
    );
  }
}

