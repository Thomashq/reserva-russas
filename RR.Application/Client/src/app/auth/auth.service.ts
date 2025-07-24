import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap, map } from 'rxjs/operators';
import { Router } from '@angular/router';
import { Account } from '../domain/models/account';
import { LoginRequest } from '../domain/dto/request/LoginRequest';
import { LoginResponse } from '../domain/dto/response/LoginResponse';
import { RegisterRequest } from '../domain/dto/request/RegisterRequest';
import { AccountCreatedResponse, AccountResponse } from '../domain/dto/response/AccountResponse';
import { RefreshTokenRequest } from '../domain/dto/request/TokenRequest';
import { RefreshTokenResponse } from '../domain/dto/response/TokenResponse';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'https://localhost:7099/api/v1/Auth';
  private currentUserSubject = new BehaviorSubject<Account | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(
    private http: HttpClient,
    private router: Router
  ) {
    const token = localStorage.getItem('token');
    if (token && this.isTokenValid(token)) {
      const user = this.getUserFromToken(token);
      if (user) {
        this.setCurrentUser(user);
      }
    } else {
      localStorage.removeItem('token');
    }
  }

  login(credentials: LoginRequest): Observable<LoginResponse> {
    // O interceptor já vai desembrulhar a ApiResponse automaticamente
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, credentials)
      .pipe(
        tap(loginResponse => {
          localStorage.setItem('token', loginResponse.token);
          // Converte AccountResponse para Account
          //const account = this.mapAccountResponseToAccount(loginResponse.account);
          //this.setCurrentUser(account);
        })
      );
  }

  register(registerData: RegisterRequest): Observable<AccountCreatedResponse> {
    // O interceptor já vai desembrulhar a ApiResponse automaticamente
    return this.http.post<AccountCreatedResponse>(`${this.apiUrl}/register`, registerData)
      .pipe(
        tap(response => {
          console.log("Registro efetuado com sucesso");
        })
      );
  }

  refreshToken(): Observable<RefreshTokenResponse> {
    const currentToken = this.getToken();
    if (!currentToken) {
      throw new Error('Nenhum token disponível para renovação');
    }

    const refreshRequest: RefreshTokenRequest = { token: currentToken };

    // O interceptor já vai desembrulhar a ApiResponse automaticamente
    return this.http.post<RefreshTokenResponse>(`${this.apiUrl}/refresh-token`, refreshRequest)
      .pipe(
        tap(response => {
          localStorage.setItem('token', response.token);
          const user = this.getUserFromToken(response.token);
          if (user) {
            this.setCurrentUser(user);
          }
        })
      );
  }

  loginWithGoogle(): void {
    console.log('Login com Google - implementar integração');
  }

  logout(): void {
    localStorage.removeItem('token');
    this.currentUserSubject.next(null);
    this.router.navigate(['/auth/login']);
  }

  isAuthenticated(): boolean {
    const token = localStorage.getItem('token');
    if(token != null)
      return token ? this.isTokenValid(token) : false;
    return false;
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  getCurrentUser(): Account | null {
    return this.currentUserSubject.value;
  }

  private setCurrentUser(user: Account): void {
    this.currentUserSubject.next(user);
  }

  private isTokenValid(token: string): boolean {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const exp = payload.exp * 1000;
      return Date.now() < exp;
    } catch (error) {
      console.error('Erro ao validar token:', error);
      return false;
    }
  }

  private getUserFromToken(token: string): Account | null {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return {
        id: parseInt(payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] || payload.nameid),
        userName: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] || payload.unique_name,
        mail: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] || payload.email,
        phone: payload.phone || undefined,
        accountPermission: parseInt(payload.permission || '0'),
        createdAt: new Date(payload.createdAt || Date.now()),
        updatedAt: new Date(payload.updatedAt || Date.now()),
        isActive: payload.isActive === 'true' || payload.isActive === true
      } as Account;
    } catch (error) {
      console.error('Erro ao decodificar token:', error);
      return null;
    }
  }

  /**
   * Converte AccountResponse (da API) para Account (modelo local)
   */
  private mapAccountResponseToAccount(accountResponse: AccountResponse): Account {
    return {
      id: accountResponse.id,
      userName: accountResponse.userName,
      mail: accountResponse.mail,
      phone: accountResponse.phone,
      accountPermission: accountResponse.accountPermission,
      isActive: accountResponse.isActive
    };
  }
}
