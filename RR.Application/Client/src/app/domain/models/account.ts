import { BaseModel } from "./base-model";

export interface Account extends BaseModel {
  userId?: number;
  userName?: string;
  passwordHash?: string;
  mail?: string;
  phone?: string | null;
  accountPermission?: number;
  isActive?: boolean;
}
