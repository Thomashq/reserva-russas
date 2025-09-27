export interface AccountDTO {
  id?: number; 
  userName?: string | null;
  password?: string | null;
  email?: string | null;
  phone?: string | null;
  accountPermission: number;
}
