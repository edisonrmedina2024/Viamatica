import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../env/enviroment';
import { RoleDTO } from '../interfaces/interfaces';

@Injectable({
  providedIn: 'root'
})
export class RoleService {
  
  private apiUrl = `${environment.apiUrl}/Role`;
  
  constructor(private http: HttpClient) {}

  getRoles(): Observable<RoleDTO[]> {
    return this.http.get<RoleDTO[]>(this.apiUrl);
  }

  getRoleById(id: number): Observable<RoleDTO> {
    return this.http.get<RoleDTO>(`${this.apiUrl}/${id}`);
  }
  getRoleByUsername(username: string): Observable<RoleDTO> {
    return this.http.get<RoleDTO>(`${this.apiUrl}/menu/${username}`);
  }
}
