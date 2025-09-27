export interface PeriodRequest {
  start: string; // enviar 'YYYY-MM-DDTHH:mm:ss' (sem Z)
  end: string;   // enviar 'YYYY-MM-DDTHH:mm:ss' (sem Z)
}

export interface CreateReservationRequest {
  roomId: number;
  accountId: number;
  title: string;
  description?: string;
  startTime: string
  endTime: string;
}

export interface UpdateReservationRequest {
  Id: number;
  roomId: number;
  accountId: number;
  title: string;
  description?: string;
  startTime: string
  endTime: string;
  note: string;
}
