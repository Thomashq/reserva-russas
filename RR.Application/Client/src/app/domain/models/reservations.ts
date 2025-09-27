import { BaseModel } from "./base-model";

export interface Reservations extends BaseModel{
  roomId: number;
  accountId: number;
  title: string;
  description?: string;
  seriesId?: number | null;
  origin?: number; // EReservationOrigin
  status?: number; // 0=criado 1=aprovado 2=rejeitado/cancelado
  startTime: string; // enviar 'YYYY-MM-DDTHH:mm:ss' (sem Z)
  endTime: string;
}

export interface ReservationsSeries extends BaseModel {
  roomId: number;
  accountId: number;
  title: string;
  description?: string;
  windowStart: string; // enviar 'YYYY-MM-DDTHH:mm:ss' (sem Z)
  windowEnd: string; // enviar 'YYYY-MM-DDTHH:mm:ss' (sem Z)
  recurrenceRule: string; // iCal RRULE
  daysOfWeek: string; // e.g. 'MO,TU,WE,TH,FR'
  timeStart: string; // enviar 'HH:mm:ss'
  timeEnd: string; // enviar 'HH:mm:ss'
  seriesStatus?: number; // 0=criado 1=aceito
}
