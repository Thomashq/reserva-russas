import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { BehaviorSubject, Observable, tap } from "rxjs";
import { LoginRequest } from "../domain/dto/request/LoginRequest";
import { RegisterRequest } from "../domain/dto/request/RegisterRequest";
import { RefreshTokenRequest } from "../domain/dto/request/TokenRequest";
import { AccountCreatedResponse, AccountResponse } from "../domain/dto/response/AccountResponse";
import { LoginResponse } from "../domain/dto/response/LoginResponse";
import { RefreshTokenResponse } from "../domain/dto/response/TokenResponse";
import { Account } from "../domain/models/account";

@Injectable({ providedIn: 'root' })
export class AuthService {
  // se tiver environment.apiBase, faça: `${environment.apiBase}/auth`
  private apiUrl = 'https://localhost:7099/auth';

  private currentUserSubject = new BehaviorSubject<Account | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient, private router: Router) {
    const token = localStorage.getItem('token');
    if (token && this.isTokenValid(token)) {
      // se quiser, tente buscar /auth/me aqui para sincronizar
      const user = this.getUserFromToken(token);
      if (user) this.setCurrentUser(user);
    } else {
      localStorage.removeItem('token');
    }
  }

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, credentials)
      .pipe(
        tap(res => {
          localStorage.setItem('token', res.token);
          // preferir o objeto vindo da API:
          if ((res as any).account) {
            const account = this.mapAccountResponseToAccount((res as any).account);
            this.setCurrentUser(account);
          } else {
            const user = this.getUserFromToken(res.token);
            if (user) this.setCurrentUser(user);
          }
        })
      );
  }

  register(registerData: RegisterRequest): Observable<AccountCreatedResponse> {
    return this.http.post<AccountCreatedResponse>(`${this.apiUrl}/register`, registerData)
      .pipe(tap(() => console.log('Registro efetuado com sucesso')));
  }

  refreshToken(): Observable<RefreshTokenResponse> {
    const currentToken = this.getToken();
    if (!currentToken) throw new Error('Nenhum token disponível para renovação');

    const refreshRequest: RefreshTokenRequest = { token: currentToken };
    return this.http.post<RefreshTokenResponse>(`${this.apiUrl}/refresh-token`, refreshRequest)
      .pipe(
        tap(res => {
          localStorage.setItem('token', res.token);
          const user = this.getUserFromToken(res.token);
          if (user) this.setCurrentUser(user);
        })
      );
  }

  logout(): void {
    localStorage.removeItem('token');
    this.currentUserSubject.next(null);
    this.router.navigate(['/auth/login']);
  }

  isAuthenticated(): boolean {
    const token = this.getToken();
    return !!token && this.isTokenValid(token);
  }

  getToken(): string | null { return localStorage.getItem('token'); }
  getCurrentUser(): Account | null { return this.currentUserSubject.value; }
  private setCurrentUser(user: Account) { this.currentUserSubject.next(user); }

  private isTokenValid(token: string): boolean {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return Date.now() < (payload.exp * 1000);
    } catch { return false; }
  }

  private getUserFromToken(token: string): Account | null {
    try {
      const p = JSON.parse(atob(token.split('.')[1]));
      return {
        id: parseInt(p['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] ?? p.nameid),
        userName: p['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] ?? p.unique_name,
        mail: p['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] ?? p.email,
        phone: p.phone ?? undefined,
        accountPermission: parseInt(p.permission ?? '0'),
        isActive: (p.isActive === 'true') || (p.isActive === true),
        // createdAt/updatedAt não vêm no token — não preencha aqui
      } as Account;
    } catch { return null; }
  }

  private mapAccountResponseToAccount(a: AccountResponse): Account {
    return {
      id: a.id,
      userName: a.userName,
      mail: a.mail,
      phone: a.phone,
      accountPermission: a.accountPermission,
      isActive: a.isActive
    };
  }
}
