import { Routes } from '@angular/router';
import { LoginComponent } from '../components/login/login.component';
import { UserTableComponent } from '../components/user-table/user-table.component';
import { authGuard } from '../guards/auth.guard';
import { DashboardComponent } from '../pages/dashboard/dashboard.component';
import { LayoutComponent } from '../pages/layout/layout.component';
import { ProfileComponent } from '../pages/profile/profile.component';
import { RecoveryComponent } from '../pages/recovery/recovery.component';
import { UnauthorizedComponent } from '../pages/unauthorized/unauthorized.component';

export const routes: Routes = [
  { path: '', component: LoginComponent },
  {
    path: 'home',
    component: LayoutComponent,
    children: [
      {
        path: 'dashboard',
        component: DashboardComponent,
        canActivate: [authGuard],
      },
      {
        path: 'profile',
        component: ProfileComponent,
        canActivate: [authGuard],
      },
      {
        path: 'forgot-password',
        component: RecoveryComponent,
        canActivate: [authGuard],
      },
      {
        path: 'manage-users',
        component: UserTableComponent,
        canActivate: [authGuard],
      },
    ],
  },
  {
    path:  'unauthorized',
    component:UnauthorizedComponent,
   },
  { path: '**', redirectTo: '' },

];
