import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class TokenService {

  hasToken(key: string) {
    return !!this.getToken(key);
  }

  setToken(key: string, token: string) {
    window.localStorage.setItem(key, token);
  }

  getToken(key: string) {
    return window.localStorage.getItem(key);
  }

  setTokenExpire(key: string, token: string) {
    window.localStorage.setItem(key, token);
  }

  getTokenExpire(key: string) {
    return window.localStorage.getItem(key);
  }

  public getTokenExpired(): boolean {
    if (this.hasToken('auth_token')) {
      let token = JSON.parse(atob(this.getToken("auth_token")!.split('.')[1]));
      let expiry = token.exp;
      return (Math.floor((new Date).getTime() / 1000)) >= expiry;
    } else {
      return true;
    }
  }

  public getTokenExpireAt(): number {
    const expiry = (JSON.parse(atob(this.getToken("auth_token")!.split('.')[1]))).exp * 1000;
    // console.log("Expira em :" + new Date(expiry))
    return expiry;
  }

  public getIssuedAt(): number {
    const issueAt = (JSON.parse(atob(this.getToken('auth_token')!.split('.')[1]))).iat * 1000;
    return issueAt;
  }

  removeToken(key: string) {
    this.hasToken(key) && window.localStorage.removeItem(key);
  }

  public getExpiredToken() {
    return JSON.parse(atob(this.getToken("auth_token")!.split('.')[1]));
  }
}
