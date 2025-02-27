export interface LoginDTO {
  credencial: string; // Puede ser email o username
  password: string;
}

export interface LoginResponseDTO {
  mensaje: string;
  exito: number;
}


export interface RoleDTO {
  idRol: number;
  rolName: string;
  rolOpcionesIdOpcions: RolOpcione[];
}

export interface RolOpcione {
  idOpcion: number;
  nombreOpcion: string;
  route: string;
}
