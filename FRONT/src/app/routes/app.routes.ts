import { Routes } from '@angular/router';
import { LoginComponent } from '../components/login/login.component';
import { UserTableComponent } from '../components/user-table/user-table.component';
import { DashboardComponent } from '../pages/dashboard/dashboard.component';
import { LayoutComponent } from '../pages/layout/layout.component';
import { ProfileComponent } from '../pages/profile/profile.component';
import { RecoveryComponent } from '../pages/recovery/recovery.component';

export const routes: Routes = [
  { path: '', component: LoginComponent },
  {
    path: 'home',
    component: LayoutComponent,
    children: [
      { path: 'dashboard', component: DashboardComponent },
      { path: 'profile', component: ProfileComponent },
      { path: 'forgot-password', component: RecoveryComponent},
      { path: 'manage-users', component: UserTableComponent },
    ],
  },
  { path: '**', redirectTo: 'login' },
];
