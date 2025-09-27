export interface CreateSeriesRequest {
  accountId: number;
  defaultRoomId: number;
  title: string;
  description?: string;
  windowStart: string; // 'YYYY-MM-DDT00:00:00'
  windowEnd: string;   // 'YYYY-MM-DDT00:00:00'
  recurrenceRule: string; // opcional no fluxo atual (pode mandar vazio)
  daysOfWeek: string;  // "MO,WE,FR"
  timeStart: string;   // "HH:mm:ss"
  timeEnd: string;     // "HH:mm:ss"
  interval: number;    // 1,2,...
}

export interface PreviewSeriesRequest extends Omit<CreateSeriesRequest, 'defaultRoomId'> {
  defaultRoomId: number;
}
