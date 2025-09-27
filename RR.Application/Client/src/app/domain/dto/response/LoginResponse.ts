import { AccountResponse } from "./AccountResponse";

export interface LoginResponse {
  token: string;
  expiresAt: string; // ISO date string
  account: AccountResponse;
}
