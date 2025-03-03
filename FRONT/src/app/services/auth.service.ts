import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../env/enviroment';
import { ActualizarUsuarioDto, CrearUsuarioDto, LoginDTO, LoginResponseDTO, ProfileDTO, RecoveryPasswordDTO } from '../interfaces/interfaces';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
 
  private apiUrl = `${environment.apiUrl}/UsuarioContoller`;

  constructor(private http: HttpClient) {}

  login(credentials: LoginDTO): Observable<LoginResponseDTO> {
    return this.http.post<LoginResponseDTO>(`${this.apiUrl}/login`, credentials);
  }

  logout(credencial: LoginDTO): Observable<LoginResponseDTO> {
    return this.http.post<LoginResponseDTO>(`${this.apiUrl}/logout`, credencial);
  }

  getDashboardStats(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/statics`);
  }

  getProfile(username: string): Observable<ProfileDTO> {
    return this.http.get<ProfileDTO>(`${this.apiUrl}/profile?username=${username}`);
  }

  createUser(user: CrearUsuarioDto ) {
    return this.http.post<CrearUsuarioDto>(
      `${this.apiUrl}/UsuarioContoller`,
      user
    );
  }

  updateProfile(profile: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/profile`, profile);
  }

  updatePassword(password: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/password`, password);
  }

  recoveryPassword(data: RecoveryPasswordDTO): Observable<boolean> {
    return this.http.post<boolean>(`${this.apiUrl}/recovery-password`, data);
  }

  getUsers(): Observable<ActualizarUsuarioDto[]> {
    return this.http.get<ActualizarUsuarioDto[]>(`${this.apiUrl}/`);
  }

}
