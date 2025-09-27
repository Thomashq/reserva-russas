import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Reservations, ReservationsSeries } from "../../domain/models/reservations";
import { CreateSeriesRequest, PreviewSeriesRequest } from "../../domain/dto/request/CreateSeriesRequest";
import { EditSeriesRequest } from "../../domain/dto/request/ReservationSeries";

@Injectable({ providedIn: 'root' })

export class ReservationSeriesService {
  constructor(
    public http: HttpClient,
    @Inject('BASE_URL') private baseUrlApi: string
  ) { }

  CancelSeries(seriesId: number, from: string) {
    return this.http.delete<ReservationsSeries>(this.baseUrlApi + 'reservationseries/' + seriesId);
  }

  CreateSeries(parameter: CreateSeriesRequest) {
    return this.http.post<number>(this.baseUrlApi + 'reservationseries', parameter);
  }

  EditSeries(parameter: EditSeriesRequest) {
    return this.http.put<ReservationsSeries>(this.baseUrlApi + 'reservationseries', parameter);
  }

  PreviewSeries(parameter: PreviewSeriesRequest) {
    return this.http.post<Reservations[]>(this.baseUrlApi + 'reservationseries/preview', parameter);

  }
}
