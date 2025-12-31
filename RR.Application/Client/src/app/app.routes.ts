import { Routes } from '@angular/router';
import { HomePageComponent } from './home-page/home-page.component';
import { LoginComponent } from './auth/auth-login/auth-login.component';
import { RegisterComponent } from './auth/auth-register/auth-register.component';
import { authGuard, noAuthGuard } from './auth/guards';

import { RoomListComponent } from './rooms/room-list/room-list.component';
import { RoomDetailsComponent } from './rooms/room-details/room-details.component';

import { AdvisorComponent } from './advisor/advisor';
import { AdvisorNewComponent } from './advisor/advisor-new/advisor-new';
import {servantOrAboveGuard}  from './auth/permission-guard';

export const routes: Routes = [
  { path: '', component: HomePageComponent, pathMatch: 'full' },

  { path: 'auth', children: [
      { path: 'login', component: LoginComponent, canActivate: [noAuthGuard] },
      { path: 'register', component: RegisterComponent, canActivate: [noAuthGuard] },
      { path: '', redirectTo: 'login', pathMatch: 'full' }
    ]
  },

  {
    path: 'room', children: [
      { path: 'list', component: RoomListComponent, canActivate: [authGuard] },
      { path: 'detail/:id', component: RoomDetailsComponent, canActivate: [authGuard] },
      { path: '', redirectTo: 'list', pathMatch: 'full' }
    ]
  },

{
  path: 'advisor', children: [
    { path: 'list', component: AdvisorComponent, canActivate: [authGuard, servantOrAboveGuard] },
    { path: 'new', component: AdvisorNewComponent, canActivate: [authGuard, servantOrAboveGuard] },
    { path: '', redirectTo: 'list', pathMatch: 'full' }
  ]
},

  { path: '**', redirectTo: '' }
];

