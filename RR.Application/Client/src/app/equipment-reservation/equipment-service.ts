import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { Equipment } from "../domain/models/equipment";

@Injectable({ providedIn: 'root' })
export class EquipmentService {
  private baseUrl: string;

  constructor(
    public http: HttpClient,
    @Inject('BASE_URL') baseUrl: string
  ) {
    this.baseUrl = baseUrl + 'equipment/';
  }

  GetAll(): Observable<Equipment[]> {
    return this.http.get<Equipment[]>(this.baseUrl);
  }

  GetById(id: number): Observable<Equipment> {
    return this.http.get<Equipment>(this.baseUrl + id);
  }

  Create(equipment: Partial<Equipment>): Observable<Equipment> {
    return this.http.post<Equipment>(this.baseUrl, equipment);
  }

  Update(id: number, equipment: Equipment): Observable<Equipment> {
    return this.http.put<Equipment>(this.baseUrl + id, equipment);
  }

  Delete(id: number): Observable<boolean> {
    return this.http.delete<boolean>(this.baseUrl + id);
  }
}
