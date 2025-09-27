export interface ApiResponse<T> {
  StatusCode: number;
  Success: boolean;
  Message: string;
  ApiVersion: string;
  ExecutedIn: string;
  Data: T;
  Errors: string[];
}
