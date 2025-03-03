import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { MenuModule } from 'primeng/menu';
import { MessageModule } from 'primeng/message';
import { MessagesModule } from 'primeng/messages';
import { SidebarModule } from 'primeng/sidebar';
import { ToolbarModule } from 'primeng/toolbar';

import { AuthService } from '../../services/auth.service';
@Component({
  selector: 'app-dashboard',
  imports: [CommonModule,
    CardModule,
    ToolbarModule,
    ButtonModule,
    MessagesModule,
    MessageModule,
    SidebarModule,
    MenuModule,
    CardModule
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css',
  providers: [MessageService]
})

export class DashboardComponent {
  usuariosActivos: number = 0;
  usuariosBloqueados: number = 0;
  loginFallidos: number = 0;
  usuariosInactivos: number = 0;
  errorMessage: string = "";
  menuItems: any[] = [];

  constructor(
    private authService: AuthService,
    private router: Router,
  ) {}

  ngOnInit() {
    const menuOptions = JSON.parse(localStorage.getItem('menuOptions') || '[]');

    this.menuItems = menuOptions.map((route: string) => ({
      label: route.replace("/",""),  
      icon: 'pi pi-home',  
      routerLink: route
    }));

    this.obtenerEstadisticasUsuarios();
  }

  
  obtenerEstadisticasUsuarios() {
    this.authService.getDashboardStats().subscribe({
      next: (data) => {
        this.usuariosActivos = data.activeUsers;
        this.usuariosInactivos = data.inactiveUsers;
        this.usuariosBloqueados = data.blockedUsers;
        this.loginFallidos = data.failedLogins;
      },
      error: (err) => {
        this.errorMessage = 'Error al obtener las estadísticas del dashboard.';
        console.error(err);
      },
    });
  }

}
