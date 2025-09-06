export interface RegisterRequest {
  fullName: string;
  userName: string;
  mail: string;
  password: string;
  phone?: string;
  //accountPermission: number;
}
