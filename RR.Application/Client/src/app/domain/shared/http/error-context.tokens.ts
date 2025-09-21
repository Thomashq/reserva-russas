import { HttpContextToken } from '@angular/common/http';

export const SKIP_GLOBAL_ERROR_HANDLER = new HttpContextToken<boolean>(() => false);
export const SKIP_ERROR_TOAST = new HttpContextToken<boolean>(() => false);
export const SKIP_AUDIT_LOG = new HttpContextToken<boolean>(() => false);
export const SKIP_BEARER_TOKEN = new HttpContextToken<boolean>(() => false);

// opcional: definir mensagem custom por requisição
export const CUSTOM_ERROR_MESSAGE = new HttpContextToken<string | null>(() => null);
