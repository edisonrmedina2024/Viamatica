import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { ToolbarModule } from 'primeng/toolbar';
import { AuthService } from '../../services/auth.service';
@Component({
  selector: 'app-header',
  imports: [
    ToolbarModule,
    ButtonModule
  ],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent implements OnInit {

  errorMessage: string = "";

  constructor(
      private authService: AuthService,
          private router: Router,
    ) {}
  ngOnInit(): void {
    
  }
    

  cerrarSesion() {
    debugger;
    const user = localStorage.getItem('user');
    
    if (user) {
      const parsedUser = JSON.parse(user);
      localStorage.removeItem('user');
      localStorage.removeItem('role');
      localStorage.removeItem('menuOptions');
      localStorage.removeItem('token');
      this.authService.logout({ credencial: parsedUser, password: '' }).subscribe({
        next: (response) => {
          if (response.exito === 1) {
            this.router.navigate(['/']);
          } else {
            this.errorMessage = response.mensaje;
          }
        },
        error: () => {
          this.errorMessage = 'Error al intentar cerrar sesión.';
        }
      });
    } else {
      this.router.navigate(['/']);
    }
    
  }

  toggleAbrir() {
    throw new Error('Method not implemented.');
  }

}
