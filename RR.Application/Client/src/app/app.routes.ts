import { Routes } from '@angular/router';
import { HomePageComponent } from './home-page/home-page.component';
import { LoginComponent } from './auth/auth-login/auth-login.component';
import { RegisterComponent } from './auth/auth-register/auth-register.component';
import { authGuard, noAuthGuard } from './auth/guards';

export const routes: Routes = [
  { path: '', component: HomePageComponent, pathMatch: 'full' },
  { path: 'auth', children: [
      { path: 'login', component: LoginComponent, canActivate: [noAuthGuard] },
      { path: 'register', component: RegisterComponent, canActivate: [noAuthGuard] },
      { path: '', redirectTo: 'login', pathMatch: 'full' }
    ]
  },
  // fallback
  { path: '**', redirectTo: '' }
];