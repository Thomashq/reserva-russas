export interface CreateSeriesRequest {
  accountId: number;
  defaultRoomId: number;
  title: string;
  description?: string;
  windowStart: string; // enviar 'YYYY-MM-DDTHH:mm:ss' (sem Z)
  windowEnd: string; // enviar 'YYYY-MM-DDTHH:mm:ss' (sem Z)
  recurrenceRule: string; // iCal RRULE
  daysOfWeek: string; // e.g. 'MO,TU,WE,TH,FR'
  timeStart: string; // enviar 'HH:mm:ss'
  timeEnd: string; // enviar 'HH:mm:ss'
  interval?: number; // opcional, default 1
}

export interface PreviewSeriesRequest extends CreateSeriesRequest{ }

export interface EditSeriesRequest {
  seriesId: number;
  roomId: number;
  title: string;
  description?: string;
  windowStart: string; // enviar 'YYYY-MM-DDTHH:mm:ss' (sem Z)
  windowEnd: string; // enviar 'YYYY-MM-DDTHH:mm:ss' (sem Z)
  recurrenceRule: string; // iCal RRULE
  daysOfWeek: string; // e.g. 'MO,TU,WE,TH,FR'
  timeStart: string; // enviar 'HH:mm:ss'
  timeEnd: string; // enviar 'HH:mm:ss'
  interval?: number; // opcional, default 1
}
