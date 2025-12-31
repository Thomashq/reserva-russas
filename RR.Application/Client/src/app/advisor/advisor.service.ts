import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { IStudentAdvisor } from "../domain/models/studentadvisor";
import { ICreateStudentAdvisorRequest} from "../domain/dto/request/StudentAdvisorRequest";

@Injectable({ providedIn: 'root' })
export class AdvisorService {

  constructor(
    public http: HttpClient,
    @Inject('BASE_URL') private baseUrlApi: string
  ) { }

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

  CreateStudentAdvisor(parameter:ICreateStudentAdvisorRequest) {
    return this.http.post<boolean>(this.baseUrlApi + 'StudentAdvisor', parameter);
  }

  UpdateStudentAdvisor(parameter:IStudentAdvisor ) {
    // teu controller atual é [HttpPut] sem rota com id
    return this.http.put<IStudentAdvisor>(this.baseUrlApi + 'StudentAdvisor', parameter);
  }

  DeleteStudentAdvisor(id: number) {
    // teu controller usa PUT delete/{id}
    return this.http.put<boolean>(this.baseUrlApi + 'StudentAdvisor/delete/' + id, {});
  }
}

