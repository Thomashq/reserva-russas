import { HttpClient, HttpContext } from "@angular/common/http";
import { Injectable, Inject} from "@angular/core";
import { Router } from "@angular/router";
import { jwtDecode } from 'jwt-decode';
import { BehaviorSubject, Observable, tap } from "rxjs";
import { LoginRequest } from "../domain/dto/request/LoginRequest";
import { RegisterRequest } from "../domain/dto/request/RegisterRequest";
import { AccountCreatedResponse } from "../domain/dto/response/AccountResponse";
import { Account } from "../domain/models/account";
import { TokenService } from "./token.service";
import { SKIP_ERROR_TOAST } from "../domain/shared/http/error-context.tokens";

@Injectable({ providedIn: 'root' })
export class AuthService {
  private url_controller: string;
  private currentUserSubject = new BehaviorSubject<Account | null>(null);
  private readonly key = 'auth_token';
  private account: Account = {}
  private loginRequest: LoginRequest = {};

  constructor(
    @Inject('BASE_URL') private apiUrl: string,
    private http: HttpClient,
    private router: Router,
    private tokenService: TokenService)
  {
    this.url_controller = `${this.apiUrl}auth`;
    this.tokenService.hasToken(this.key) &&
      this.decodeAndNotify();
  }

  private decodeAndNotify() {
    const token = this.tokenService.getToken(this.key);

    const tokenDecoded = jwtDecode(token!);
    const acc = (tokenDecoded as any).account;

    this.account = JSON.parse(acc as string) as Account

    this.currentUserSubject.next(this.account);
  }

  Login(login: LoginRequest) {
    localStorage.removeItem('auth_token');
    const ctx = new HttpContext()
      .set(SKIP_ERROR_TOAST, true);

    return this.http.post<string>(this.url_controller + "/login", login, {
      observe: 'response',
      context: ctx
    }).pipe(tap(res => {
      const authToken = res.body;
      this.setToken(authToken!.toString());
    }));
  }


  register(registerData: RegisterRequest): Observable<AccountCreatedResponse> {
    return this.http.post<AccountCreatedResponse>(this.url_controller + '/register', registerData)
      .pipe(tap(() => console.log('Registro efetuado com sucesso')));
  }

  setToken(token: string) {
    this.tokenService.setToken(this.key, token);
    this.decodeAndNotify();
  }

  get Account() {
    return this.account;
  }

  isLoggedIn() {
    return this.tokenService.hasToken(this.key);
  }

  logout(): void {
    localStorage.removeItem('auth_token');
    this.currentUserSubject.next(null);
    this.account = {};
    this.router.navigate(['/auth/login']);
  }

  refreshToken() {
    this.http.post<string>(this.url_controller + "refresh-token", null, { observe: 'response' }).subscribe(res => {
      const authToken = res.body;
      this.setToken(authToken!.toString());
    });
  }
}
