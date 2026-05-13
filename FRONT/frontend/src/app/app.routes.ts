import { Routes } from '@angular/router';
import { authGuard, guestGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/dashboard',
    pathMatch: 'full'
  },
  {
    path: 'dashboard',
    loadComponent: () =>
      import('./features/dashboard/pages/dashboard/dashboard.component').then(
        m => m.DashboardComponent
      ),
    canActivate: [authGuard],
    title: 'Dashboard - Gestão de Títulos'
  },
  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/pages/login/login.component').then(m => m.LoginComponent),
    canActivate: [guestGuard],
    title: 'Login - Paschoalotto'
  },
  {
    path: 'register',
    loadComponent: () =>
      import('./features/auth/pages/register/register.component').then(m => m.RegisterComponent),
    canActivate: [guestGuard],
    title: 'Cadastro - Paschoalotto'
  },
  {
    path: 'titulos',
    loadComponent: () =>
      import('./features/titulos/pages/titulo-list/titulo-list.component').then(
        m => m.TituloListComponent
      ),
    canActivate: [authGuard],
    title: 'Títulos Cadastrados - Gestão de Títulos'
  },
  {
    path: 'titulos/novo',
    loadComponent: () =>
      import('./features/titulos/pages/titulo-form/titulo-form.component').then(
        m => m.TituloFormComponent
      ),
    canActivate: [authGuard],
    title: 'Cadastrar Título - Gestão de Títulos'
  },
  {
    path: '**',
    redirectTo: '/dashboard'
  }
];
