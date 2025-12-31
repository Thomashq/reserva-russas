export enum EAccountPermission {
  Manager = 0,
  Servant = 1,
  Student = 2,
  Admin = 3
}

export function getPermissionLevel(p: number): number {
  // hierarquia desejada: Student < Servant < Manager < Admin
  // o thomas é idiota e nao fez na ordem certa quando era pra ter feito, entao converte assim mesmo
  switch (p) {
    case EAccountPermission.Student: return 0;
    case EAccountPermission.Servant: return 1;
    case EAccountPermission.Manager: return 2;
    case EAccountPermission.Admin: return 3;
    default: return 0;
  }
}

