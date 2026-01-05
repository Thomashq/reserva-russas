import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Reservations } from "../../domain/models/reservations";
import { CreateReservationRequest, PeriodRequest, UpdateReservationRequest } from "../../domain/dto/request/ReservationRequest";

@Injectable({ providedIn: 'root' })

export class ReservationService {
  constructor(
    public http: HttpClient,
    @Inject('BASE_URL') private baseUrlApi: string
  ) { }

  GetAllReservations() {
    return this.http.get<Reservations[]>(this.baseUrlApi + 'reservation/');
  }

  GetReservationById(id: number) {
    return this.http.get<Reservations>(this.baseUrlApi + 'reservation/' + id);
  }

  GetReservationsByRoomId(roomId: number) {
    return this.http.get<Reservations[]>(this.baseUrlApi + 'reservation/room/' + roomId);
  }

  GetReservationsByAccountId(accountId: number) {
    return this.http.get<Reservations[]>(this.baseUrlApi + 'reservation/account/' + accountId);
  }

  GetReservationsByPeriod(parameter: PeriodRequest) {
    return this.http.post<Reservations[]>(this.baseUrlApi + 'reservation/period', parameter);
  }

  CreateReservation(parameter: CreateReservationRequest) {
    return this.http.post<Reservations>(this.baseUrlApi + 'reservation', parameter);
  }

  UpdateReservation(id: number, parameter: UpdateReservationRequest) {
    return this.http.put<Reservations>(this.baseUrlApi + 'reservation/' + id, parameter);
  }

  DeleteReservation(id: number) {
    return this.http.delete(this.baseUrlApi + 'reservation/' + id);
  }

  ApproveReservation(id: number) {
    return this.http.post(this.baseUrlApi + 'reservation/approve', + id);
  }

  CancelReservation(id: number) {
    return this.http.post(this.baseUrlApi + 'reservation/cancel', + id);
  }
}
