import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { DataViewModule } from 'primeng/dataview';
import { InputTextModule } from 'primeng/inputtext';
import { ProfileDTO } from '../../interfaces/interfaces';
import { AuthService } from '../../services/auth.service';
@Component({
  selector: 'app-profile',
  imports: [
    CommonModule,
    FormsModule,
    CardModule,
    ButtonModule,
    InputTextModule,
    ButtonModule,
    DataViewModule,
  ],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css',
  standalone: true,
})
export class ProfileComponent {
  loading: boolean = false;

  profile: ProfileDTO = {
    usuario: '',
    email: '',
    intentoFallidos: 0,
    ultimaSession: '',
    ultimaSessionInicio: '',
    ultimaSessionFin: '',
  };

  constructor(private profileService: AuthService) {}

  ngOnInit() {
    this.getProfile();
  }

  getProfile() {
    debugger;
    this.loading = true;

    const username = localStorage.getItem('user');

    if (!username) {
      console.error('No se encontró el usuario en el localStorage');
      this.loading = false;
      return;
    }

    this.profileService
      .getProfile(username.replace('"', '').replace('"', ''))
      .subscribe({
        next: (data) => {
          this.profile = data;
          this.loading = false;
        },
        error: (err) => {
          console.error('Error al obtener el perfil:', err);
          this.loading = false;
        },
      });
  }
}
