import { Routes } from "@angular/router";
import { LoginComponent } from "../components/login/login.component";
import { DashboardComponent } from "../pages/dashboard/dashboard.component";

const routes: Routes = [
    {
      path: 'dashboard',
      component: DashboardComponent,
      outlet: 'aux'
    },
    {
      path: '',
      component:LoginComponent ,
      outlet: 'aux'
    },
    {}
  ];