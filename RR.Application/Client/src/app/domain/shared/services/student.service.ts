import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { AccountLookup } from "../../dto/response/AccountResponse";

export interface StudentDto {
  Id: number;
  AccountId: number;
  Account?: AccountLookup | null;
  account?: AccountLookup | null;
  IsActive?: boolean;
}

@Injectable({ providedIn: 'root' })
export class StudentService {
  constructor(public http: HttpClient, @Inject('BASE_URL') private baseUrlApi: string) {}

  GetAll() {
    return this.http.get<StudentDto[]>(this.baseUrlApi + 'student/');
  }

  GetById(id: number) {
    return this.http.get<StudentDto>(this.baseUrlApi + 'student/' + id);
  }

  GetByAccountId(accountId: number) {
    return this.http.get<StudentDto>(this.baseUrlApi + 'student/account/' + accountId);
  }
}

