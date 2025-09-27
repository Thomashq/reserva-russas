import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Rooms } from "../domain/models/rooms";
import { Reservations } from "../domain/models/reservations";

@Injectable({ providedIn: 'root' })

export class RoomsService {
  constructor(
    public http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string
  ) {
    this.baseUrl = baseUrl + 'room/';
  }

  GetAll() {
    return this.http.get<Rooms[]>(this.baseUrl);
  }

  GetById(id: number) {
    return this.http.get<Rooms>(this.baseUrl + id);
  }

  GetRoomsReservationsById(id: number) {
    return this.http.get<Rooms>(this.baseUrl + 'period/' + id);
  }

  GetRoomsReservationsByPeriod(id: number, start: string, end: string) {
    return this.http.get<Reservations[]>(this.baseUrl + 'period/' + id + '?start=' + start + '&end=' + end);
  }

  AddAsync(room: Rooms) {
    return this.http.post<Rooms>(this.baseUrl, room);
  }

  DeleteAsync(id: number) {
    return this.http.delete<boolean>(this.baseUrl + id);
  }

  UpdateAsync(room: Rooms) {
    return this.http.put<Rooms>(this.baseUrl + room.Id, room);
  }
  
}
