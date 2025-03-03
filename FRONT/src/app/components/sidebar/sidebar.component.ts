import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { MenuModule } from 'primeng/menu';
import { SidebarModule } from 'primeng/sidebar';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-sidebar',
  imports: [
      SidebarModule,
      MenuModule,
      ButtonModule,
    ],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.css'
})
export class SidebarComponent {
  menuItems: any[] = [];
  errorMessage: string = "";
  sidebarVisible: boolean = false;
visibleMenu: boolean = true;

  constructor(
    private authService: AuthService,
        private router: Router,
  ) {}

  ngOnInit() {
    debugger;
    const menuOptions = JSON.parse(localStorage.getItem('menuOptions') || '[]');

    this.menuItems = menuOptions.map((route: string) => ({
      label: route.replace("/","").replace("home",""),  
      icon: 'pi pi-home', 
      routerLink: route
    }));

  }

}
