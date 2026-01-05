import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { AccountLookup } from "../../dto/response/AccountResponse";

@Injectable({ providedIn: 'root' })

export class AccountLookupService {
  constructor(public http: HttpClient, @Inject('BASE_URL') private baseUrlApi: string) {}

  Search(q: string, permission?: number, take: number = 20) {
    const qs = encodeURIComponent(q ?? '');
    const p = permission === null || permission === undefined ? '' : `&permission=${permission}`;
    return this.http.get<AccountLookup[]>(this.baseUrlApi + `Account/search?q=${qs}${p}&take=${take}`);

  }
}
