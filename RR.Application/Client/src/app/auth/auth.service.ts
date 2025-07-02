import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap, map } from 'rxjs/operators';
import { Router } from '@angular/router';
import { Account } from '../domain/models/account';
import { AccountDTO } from '../domain/dto/AccountDTO';

export interface LoginRequest {
  userName: string;
  senha: string;
}

// Interface para a resposta da API
export interface ApiResponse<T> {
  data: T;
  success: boolean;
  message: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'https://localhost:7099/api/Auth'; // Removido a barra final
  private currentUserSubject = new BehaviorSubject<Account | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(
    private http: HttpClient,
    private router: Router
  ) {
    // Verifica se há um token salvo no localStorage
    const token = localStorage.getItem('token');
    if (token && this.isTokenValid(token)) {
      const user = this.getUserFromToken(token);
      if (user) {
        this.setCurrentUser(user);
      }
    } else {
      // Remove token inválido
      localStorage.removeItem('token');
    }
  }

  login(credentials: LoginRequest): Observable<string> {
    // Usando HttpParams para enviar como query parameters (conforme seu backend)
    const params = new HttpParams()
      .set('userName', credentials.userName)
      .set('senha', credentials.senha);

    return this.http.post<ApiResponse<string>>(`${this.apiUrl}/login`, null, { params })
      .pipe(
        map(response => {
          if (response.success) {
            return response.data;
          } else {
            throw new Error(response.message);
          }
        }),
        tap(token => {
          // Salva o token no localStorage
          localStorage.setItem('token', token);

          // Extrai as informações do usuário do token JWT
          const user = this.getUserFromToken(token);
          if (user) {
            this.setCurrentUser(user);
          }
        })
      );
  }

  register(accountData: AccountDTO): Observable<boolean> {
    return this.http.post<ApiResponse<boolean>>(`${this.apiUrl}/register`, accountData)
      .pipe(
        map(response => {
          if (!response.success) {
            throw new Error(response.message);
          }
          return response.data;
        })
      );
  }

  refreshToken(): Observable<string> {
    const currentToken = this.getToken();
    if (!currentToken) {
      throw new Error('Nenhum token disponível para renovação');
    }

    return this.http.post<ApiResponse<string>>(`${this.apiUrl}/refreshtoken`, `"${currentToken}"`, {
      headers: {
        'Content-Type': 'application/json'
      }
    }).pipe(
      map(response => {
        if (response.success) {
          return response.data;
        } else {
          throw new Error(response.message);
        }
      }),
      tap(newToken => {
        localStorage.setItem('token', newToken);
        const user = this.getUserFromToken(newToken);
        if (user) {
          this.setCurrentUser(user);
        }
      })
    );
  }

  loginWithGoogle(): void {
    console.log('Login com Google - implementar integração');
    // TODO: Implementar integração com Google OAuth
  }

  logout(): void {
    localStorage.removeItem('token');
    this.currentUserSubject.next(null);
    this.router.navigate(['/auth/login']);
  }

  isAuthenticated(): boolean {
    const token = localStorage.getItem('token');
    return token ? this.isTokenValid(token) : false;
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
      const exp = payload.exp * 1000; // Convert to milliseconds
      return Date.now() < exp;
    } catch (error) {
      console.error('Erro ao validar token:', error);
      return false;
    }
  }

  private getUserFromToken(token: string): Account | null {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));

      // Mapeamento correto dos claims do JWT conforme seu backend
      return {
        id: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] || payload.nameid,
        userName: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] || payload.unique_name,
        mail: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] || payload.email,
        accountPermission: payload.permission
      } as Account;
    } catch (error) {
      console.error('Erro ao decodificar token:', error);
      return null;
    }
  }
}
