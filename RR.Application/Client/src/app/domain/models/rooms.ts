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
