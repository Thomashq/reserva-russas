import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError, BehaviorSubject } from 'rxjs';
import { catchError, switchMap, filter, take } from 'rxjs/operators';
import { AuthService } from '../auth/auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private isRefreshing = false;
  private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  constructor(private authService: AuthService) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // Adiciona o token de autorização se disponível
    const authToken = this.authService.getToken();

    if (authToken) {
      req = this.addTokenToRequest(req, authToken);
    }

    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        // Se o erro for 401 (Unauthorized) e não for uma requisição de login/refresh
        if (error.status === 401 && !req.url.includes('/login') && !req.url.includes('/refreshtoken')) {
          return this.handle401Error(req, next);
        }

        return throwError(() => error);
      })
    );
  }

  private addTokenToRequest(request: HttpRequest<any>, token: string): HttpRequest<any> {
    return request.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  private handle401Error(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.refreshTokenSubject.next(null);

      return this.authService.refreshToken().pipe(
        switchMap((newToken: string) => {
          this.isRefreshing = false;
          this.refreshTokenSubject.next(newToken);

          return next.handle(this.addTokenToRequest(request, newToken));
        }),
        catchError((error) => {
          this.isRefreshing = false;

          // Se o refresh token falhar, faz logout
          this.authService.logout();

          return throwError(() => error);
        })
      );
    }

    // Se já estiver tentando renovar o token, aguarda
    return this.refreshTokenSubject.pipe(
      filter(token => token !== null),
      take(1),
      switchMap((token) => next.handle(this.addTokenToRequest(request, token)))
    );
  }
}
