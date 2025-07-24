export interface RegisterRequest {
  userName: string;
  mail: string;
  password: string;
  phone?: string;
  accountPermission: number;
}
