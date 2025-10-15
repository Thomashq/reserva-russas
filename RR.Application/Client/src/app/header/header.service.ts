import { Injectable, Inject } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { AccountResponse } from "../domain/dto/response/AccountResponse";

@Injectable({ providedIn: 'root' })
export class HeaderService {
  private url_controller: string;

  constructor(
    @Inject('BASE_URL') private apiUrl: string,
    private http: HttpClient
  ) {
    this.url_controller = this.apiUrl;
  }

  GetAccountById(id: number) {
    return this.http.get<AccountResponse>(`${this.url_controller}account/${id}`);
  }
}
