import { BaseModel } from './base-model';

export interface Equipment extends BaseModel {
  Id: number;
  Name: string;
  Description?: string | null;
  RoomEquipments?: RoomEquipment[];
}

export interface RoomEquipment extends BaseModel {
  RoomDetailsId: number;
  EquipmentId: number;
  Quantity: number;
  RoomDetails?: RoomDetails;
  Equipment?: Equipment;
}

export interface EquipmentReservation extends BaseModel {
  Id: number;
  EquipmentId: number;
  AccountId: number;
  RoomReservationId?: number | null;
  Title: string;
  Description?: string | null;
  StartTime: Date;
  EndTime: Date;
  Status: number; // 0=Criado, 1=Aprovado, 2=Cancelado
  Equipment?: Equipment;
  Account?: any;
}

export interface RoomDetails extends BaseModel {
  Id: number;
  RoomId: number;
  IsReserveable: boolean;
  RoomType: number;
  Room?: Rooms;
  RoomEquipments?: RoomEquipment[];
  EquipmentList?: Equipment[];
}

// Re-export from rooms if needed
import { Rooms } from './rooms';
