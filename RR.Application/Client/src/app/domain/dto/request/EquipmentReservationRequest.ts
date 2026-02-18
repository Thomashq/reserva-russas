export interface CreateEquipmentReservationRequest {
  equipmentId: number;
  accountId: number;
  roomReservationId?: number;
  title: string;
  description?: string;
  startTime: string | Date;
  endTime: string | Date;
}

export interface EquipmentReservationUpdateRequest {
  id: number;
  equipmentId: number;
  accountId: number;
  startTime: string | Date;
  endTime: string | Date;
}

export interface EquipmentAvailabilityRequest {
  equipmentId: number;
  start: string | Date;
  end: string | Date;
}
