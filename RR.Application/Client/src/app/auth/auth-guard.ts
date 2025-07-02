import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../auth/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(
    private authService: AuthService,
    private router: Router
  ) { }

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | Promise<boolean> | boolean {

    if (this.authService.isAuthenticated()) {
      return true;
    }

    // Usuário não está autenticado, redireciona para login
    console.log('Usuário não autenticado, redirecionando para login');
    this.router.navigate(['/auth/login'], {
      queryParams: { returnUrl: state.url }
    });

    return false;
  }
}

// Guard para impedir acesso às páginas de auth quando já logado
@Injectable({
  providedIn: 'root'
})
export class NoAuthGuard implements CanActivate {

  constructor(
    private authService: AuthService,
    private router: Router
  ) { }

  canActivate(): boolean {
    if (!this.authService.isAuthenticated()) {
      return true;
    }

    // Usuário já está autenticado, redireciona para home
    console.log('Usuário já autenticado, redirecionando para home');
    this.router.navigate(['/']); // Redireciona para home

    return false;
  }
}
