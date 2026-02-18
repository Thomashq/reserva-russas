import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { EquipmentReservation, Equipment } from "../domain/models/equipment";
import {
  CreateEquipmentReservationRequest,
  EquipmentReservationUpdateRequest,
  EquipmentAvailabilityRequest
} from "../domain/dto/request/EquipmentReservationRequest";
import { PeriodRequest } from "../domain/dto/request/ReservationRequest";

@Injectable({ providedIn: 'root' })
export class EquipmentReservationService {
  private baseUrl: string;

  constructor(
    public http: HttpClient,
    @Inject('BASE_URL') baseUrl: string
  ) {
    this.baseUrl = baseUrl + 'equipmentreservation/';
  }

  GetAll(): Observable<EquipmentReservation[]> {
    return this.http.get<EquipmentReservation[]>(this.baseUrl);
  }

  GetById(id: number): Observable<EquipmentReservation> {
    return this.http.get<EquipmentReservation>(this.baseUrl + id);
  }

  GetByEquipment(equipmentId: number): Observable<EquipmentReservation[]> {
    return this.http.get<EquipmentReservation[]>(this.baseUrl + 'equipment/' + equipmentId);
  }

  GetByAccount(accountId: number): Observable<EquipmentReservation[]> {
    return this.http.get<EquipmentReservation[]>(this.baseUrl + 'account/' + accountId);
  }

  GetByPeriod(request: PeriodRequest): Observable<EquipmentReservation[]> {
    return this.http.post<EquipmentReservation[]>(this.baseUrl + 'period', request);
  }

  CheckAvailability(request: EquipmentAvailabilityRequest): Observable<{ available: boolean }> {
    return this.http.post<{ available: boolean }>(this.baseUrl + 'availability', request);
  }

  Create(request: CreateEquipmentReservationRequest): Observable<EquipmentReservation> {
    return this.http.post<EquipmentReservation>(this.baseUrl, request);
  }

  Update(id: number, request: EquipmentReservationUpdateRequest): Observable<EquipmentReservation> {
    return this.http.put<EquipmentReservation>(this.baseUrl + id, request);
  }

  Cancel(id: number): Observable<boolean> {
    return this.http.put<boolean>(this.baseUrl + 'cancel/' + id, {});
  }

  Approve(id: number): Observable<EquipmentReservation> {
    return this.http.put<EquipmentReservation>(this.baseUrl + 'approve/' + id, {});
  }

  Delete(id: number): Observable<boolean> {
    return this.http.delete<boolean>(this.baseUrl + id);
  }
}
