import { HttpClient, HttpContext } from "@angular/common/http";
import { Injectable, Inject } from "@angular/core";
import { Router } from "@angular/router";
import { BehaviorSubject, Observable, tap, catchError, of, map } from "rxjs";
import { LoginRequest } from "../domain/dto/request/LoginRequest";
import { RegisterRequest } from "../domain/dto/request/RegisterRequest";
import { AccountCreatedResponse } from "../domain/dto/response/AccountResponse";
import { Account } from "../domain/models/account";
import { SKIP_ERROR_TOAST } from "../domain/shared/http/error-context.tokens";

interface AuthCheckResponse {
  isAuthenticated: boolean;
  account: Account | null;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private url_controller: string;
  private currentUserSubject = new BehaviorSubject<Account | null>(null);
  private account: Account = {};
  private isInitialized = false;

  constructor(
    @Inject('BASE_URL') private apiUrl: string,
    private http: HttpClient,
    private router: Router) {
    this.url_controller = `${this.apiUrl}auth`;
    this.initializeAuth();
  }

  private async initializeAuth() {
    if (this.isInitialized) return;

    this.checkAuthStatus().subscribe({
      next: (response: any) => {
        if (response.isAuthenticated && response.account) {
          this.account = response.account;
          this.currentUserSubject.next(this.account);
        }
        this.isInitialized = true;
      },
      error: () => {
        this.isInitialized = true;
      }
    });
  }

  private checkAuthStatus(): Observable<AuthCheckResponse> {
    return this.http.get<AuthCheckResponse>(this.url_controller + "/check", {
      withCredentials: true
    }).pipe(
      catchError(() => of({ isAuthenticated: false, account: null }))
    );
  }

  Login(login: LoginRequest): Observable<any> {
    const ctx = new HttpContext()
      .set(SKIP_ERROR_TOAST, true);

    return this.http.post(this.url_controller + "/login", login, {
      observe: 'response',
      context: ctx,
      withCredentials: true
    }).pipe(
      tap(() => {
        // Após login bem-sucedido, busca os dados do usuário
        this.getCurrentUser().subscribe();
      })
    );
  }

  register(registerData: RegisterRequest): Observable<AccountCreatedResponse> {
    return this.http.post<AccountCreatedResponse>(this.url_controller + '/register', registerData, {
      withCredentials: true
    }).pipe(tap(() => console.log('Registro efetuado com sucesso')));
  }

  getCurrentUser(): Observable<Account> {
    return this.http.get<Account>(this.url_controller + "/me", {
      withCredentials: true
    }).pipe(
      tap((account: Account) => {
        this.account = account;
        this.currentUserSubject.next(this.account);
      }),
      catchError((error: any) => {
        this.currentUserSubject.next(null);
        this.account = {};
        throw error;
      })
    );
  }

  get Account() {
    return this.account;
  }

  get currentUser$(): Observable<Account | null> {
    return this.currentUserSubject.asObservable();
  }

  isLoggedIn(): boolean {
    return !!this.account && Object.keys(this.account).length > 0;
  }

  logout(): Observable<any> {
    return this.http.post(this.url_controller + "/logout", {}).pipe(
      tap(() => {
        this.currentUserSubject.next(null);
        this.account = {};
        this.router.navigate(['/auth/login']);
      }),
      catchError(() => {
        this.currentUserSubject.next(null);
        this.account = {};
        return of(null);
      })
    );
  }

  // Método para verificar se o usuário ainda está autenticado
  checkAuth(): Observable<boolean> {
    return this.checkAuthStatus().pipe(
      tap((response: any) => {
        if (response.isAuthenticated && response.account) {
          this.account = response.account;
          this.currentUserSubject.next(this.account);
        } else {
          this.account = {};
          this.currentUserSubject.next(null);
        }
      }),
      map((response: any) => response.isAuthenticated),
      catchError(() => {
        this.account = {};
        this.currentUserSubject.next(null);
        return of(false);
      })
    );
  }

}
