export interface LoginDTO {
  credencial: string; // Puede ser email o username
  password: string;
}

export interface LoginResponseDTO {
  mensaje: string;
  exito: number;
  token:string;
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

export interface ProfileDTO {
  usuario: '',
  email: '',
  intentoFallidos: 0,
  ultimaSession: '',
  ultimaSessionInicio: '',
  ultimaSessionFin: ''
};

export interface RecoveryPasswordDTO {
  username: string;
  newPassword: string;
}

export interface ActualizarUsuarioDto{
        UserName :string,
        Identificacion :string,
        Nombres:string,
        Apellidos:string,
        active:boolean,
}

export interface CrearUsuarioDto {
  userName: string;
  identificacion: string;
  nombres: string;
  apellidos: string;
  fechaNacimiento?: Date | null;
  password: string;
  active?: boolean;
}