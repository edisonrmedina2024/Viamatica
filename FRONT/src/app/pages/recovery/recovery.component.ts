import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-recovery',
  imports: [CommonModule,ReactiveFormsModule,FormsModule],
  templateUrl: './recovery.component.html',
  styleUrl: './recovery.component.css',
  standalone:  true,
})
export class RecoveryComponent {

  newPassword: string = '';
  confirmPassword: string = '';
  loading: boolean = false;

  constructor(private authService: AuthService, private router: Router) {}

  onSubmit() {
    if (this.newPassword !== this.confirmPassword) {
      alert('Las contraseñas no coinciden');
      return;
    }
  
    this.loading = true;
    const username = localStorage.getItem('user'); 
  
    if (!username) {
      alert('No se pudo encontrar el nombre de usuario.');
      this.loading = false;
      return;
    }
  
    this.authService
      .recoveryPassword({
        newPassword: this.newPassword,
        username: username.replace('"', '').replace('"', ''),
      })
      .subscribe({
        next: (result: boolean) => {
          this.loading = false;
  
          if (result) {
            this.router.navigate(['/']);
          } else {
            alert('No se pudo restablecer la contraseña. Intenta nuevamente.');
          }
        },
        error: (err) => {
          this.loading = false;
          alert('Hubo un error al restablecer la contraseña');
        },
      });
  }

}
