export interface AccountResponse {
  id: number;
  userName: string;
  mail: string;
  phone?: string;
  accountPermission: number;
  createdAt: string;
  updatedAt: string;
  isActive: boolean;
}

// Interface para resposta resumida de conta (para listagens)
export interface AccountSummaryResponse {
  id: number;
  userName: string;
  mail: string;
  accountPermission: number;
  isActive: boolean;
}

// Interface para resposta de conta criada
export interface AccountCreatedResponse {
  id: number;
  userName: string;
  mail: string;
  phone?: string;
  accountPermission: number;
  createdAt: string; // ISO date string
}

export interface AccountLookup {
  Id: number;
  UserName?: string;
  Mail?: string;
  AccountPermission: number;
}
