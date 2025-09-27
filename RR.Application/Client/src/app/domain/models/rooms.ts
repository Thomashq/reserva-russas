import { Manager } from "./manager";
import { Reservations } from "./reservations";

export interface Rooms {
  Id: number;
  Name: string;
  Capacity: number;
  ManagerId: number;
  Manager: Manager
  Reservations: Reservations[]
}
