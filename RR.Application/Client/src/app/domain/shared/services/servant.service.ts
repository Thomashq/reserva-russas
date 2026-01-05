import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { AccountLookup } from "../../dto/response/AccountResponse";

export interface ServantDto {
  Id: number;
  AccountId: number;
  Account?: AccountLookup | null;
  account?: AccountLookup | null;
  IsActive?: boolean;
}

@Injectable({ providedIn: 'root' })
export class ServantService {
  constructor(public http: HttpClient, @Inject('BASE_URL') private baseUrlApi: string) {}

  GetAll() {
    return this.http.get<ServantDto[]>(this.baseUrlApi + 'servant/');
  }

  GetById(id: number) {
    return this.http.get<ServantDto>(this.baseUrlApi + 'servant/' + id);
  }

  GetByAccountId(accountId: number) {
    return this.http.get<ServantDto>(this.baseUrlApi + 'servant/account/' + accountId);
  }
}

