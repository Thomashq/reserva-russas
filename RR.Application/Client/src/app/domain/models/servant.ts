import { BaseModel } from "./base-model";
import { Account } from "./account";

/**
 * Interface Servant - representa um servidor no sistema
 * Baseado na entidade C#: RR.Core.Entities.Servant
 */
export interface Servant extends BaseModel {
  Id: number;
  AccountId: number;
  Account?: Account | null; // Lazy loading - pode vir nulo

  // Herdado de BaseModel/BaseEntity
  CreatedAt?: Date | string;
  UpdatedAt?: Date | string;
  IsActive?: boolean;
}

/**
 * DTO simplificado para listagens e operações básicas
 * Usado nos services
 */
export interface ServantDto {
  Id: number;
  AccountId: number;
  Account?: Account | null;
  account?: Account | null; // case insensitive fallback
  IsActive?: boolean;
  isActive?: boolean; // case insensitive fallback
  CreatedAt?: Date | string;
  UpdatedAt?: Date | string;
}

