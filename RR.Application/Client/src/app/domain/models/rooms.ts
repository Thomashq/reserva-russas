import { Manager } from "./manager";
import { Reservations } from "./reservations";
import { BaseModel } from './base-model';

export interface Rooms extends BaseModel {
  Id: number;
  Name: string;
  Capacity: number;
  ManagerId: number;
  Manager: Manager
  Reservations: Reservations[]
}

export interface RoomDetails {
  Id: number;
  RoomId: number;
  IsReserveable: boolean;
  RoomType: number;

  Room?: Rooms;
  EquipmentList?: Equipment[];
}

export interface RoomEquipment {
  RoomDetailsId: number;
  EquipmentId: number;
  Quantity: number;
  Notes?: string | null;

  RoomDetails?: RoomDetails;
  Equipment?: Equipment;
}

export interface Equipment {
  Id: number;
  Name: string;
  Description?: string | null;
  RoomEquipments?: RoomEquipment[];
}

