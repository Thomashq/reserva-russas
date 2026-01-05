import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { IStudentAdvisor } from "../domain/models/studentadvisor";
import { ICreateStudentAdvisorRequest } from "../domain/dto/request/StudentAdvisorRequest";
import { Student } from "../domain/models/student";
import { Servant } from "../domain/models/servant";

@Injectable({ providedIn: 'root' })
export class AdvisorService {

  private baseUrlApiStudent: string;
  private baseUrlApiServant: string;

  constructor(
    public http: HttpClient,
    @Inject('BASE_URL') private baseUrlApi: string
  ) {
    this.baseUrlApiStudent = this.baseUrlApi + 'Student/';
    this.baseUrlApiServant = this.baseUrlApi + 'Servant/';
  }

  GetAllStudentAdvisors() {
    return this.http.get<IStudentAdvisor[]>(this.baseUrlApi + 'StudentAdvisor');
  }

  GetStudentAdvisorById(id: number) {
    return this.http.get<IStudentAdvisor>(this.baseUrlApi + 'StudentAdvisor/' + id);
  }

  GetStudentAdvisorsByStudentId(studentId: number) {
    return this.http.get<IStudentAdvisor[]>(this.baseUrlApi + 'StudentAdvisor/student/' + studentId);
  }

  GetStudentAdvisorsByServantId(servantId: number) {
    return this.http.get<IStudentAdvisor[]>(this.baseUrlApi + 'StudentAdvisor/servant/' + servantId);
  }

  CreateStudentAdvisor(parameter: ICreateStudentAdvisorRequest) {
    return this.http.post<boolean>(this.baseUrlApi + 'StudentAdvisor', parameter);
  }

  UpdateStudentAdvisor(parameter: IStudentAdvisor) {
    return this.http.put<IStudentAdvisor>(this.baseUrlApi + 'StudentAdvisor', parameter);
  }

  DeleteStudentAdvisor(id: number) {
    return this.http.put<boolean>(this.baseUrlApi + 'StudentAdvisor/delete/' + id, {});
  }

  // NOVOS endpoints (controllers novas)
  GetStudentByAccountId(accountId: number) {
    return this.http.get<Student>(this.baseUrlApiStudent + 'account/' + accountId);
  }

  GetStudentById(id: number) {
    return this.http.get<Student>(this.baseUrlApiStudent + id);
  }

  GetServantByAccountId(accountId: number) {
    return this.http.get<Servant>(this.baseUrlApiServant + 'account/' + accountId);
  }

  GetServantById(id: number) {
    return this.http.get<Servant>(this.baseUrlApiServant + id);
  }
}
