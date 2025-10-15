import { BaseModel } from "./base-model";

export interface Account extends BaseModel {
  Id?: number;
  UserId?: number;
  UserName?: string;
  PasswordHash?: string;
  Mail?: string;
  Phone?: string | null;
  AccountPermission?: number;
  IsActive?: boolean;
}
