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
