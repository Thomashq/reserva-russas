import { BaseModel } from "./base-model";

export interface Reservations extends BaseModel{
  Id: number;
  RoomId: number;
  AccountId: number;
  Title: string;
  Description?: string;
  SeriesId?: number | null;
  Origin?: number; // EReservationOrigin
  Status?: number; // 0=criado 1=aprovado 2=rejeitado/cancelado
  StartTime: string; // enviar 'YYYY-MM-DDTHH:mm:ss' (sem Z)
  EndTime: string;
}

export interface ReservationsSeries extends BaseModel {
  Id: number;
  RoomId: number;
  AccountId: number;
  Title: string;
  Description?: string;
  WindowStart: string; // enviar 'YYYY-MM-DDTHH:mm:ss' (sem Z)
  WindowEnd: string; // enviar 'YYYY-MM-DDTHH:mm:ss' (sem Z)
  RecurrenceRule: string; // iCal RRULE
  DaysOfWeek: string; // e.g. 'MO,TU,WE,TH,FR'
  TimeStart: string; // enviar 'HH:mm:ss'
  TimeEnd: string; // enviar 'HH:mm:ss'
  SeriesStatus?: number; // 0=criado 1=aceito
}
