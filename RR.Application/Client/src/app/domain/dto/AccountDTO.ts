export interface AccountDTO {
  id: string; // Guid
  userName?: string | null;
  password?: string | null;
  email?: string | null;
  phone?: string | null;
  accountPermission: number;
}
