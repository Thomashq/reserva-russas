import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomePageComponent } from './home-page/home-page.component';


import { FooterComponent } from './footer/footer.component';
import { FooterModule } from './footer/footer.module';
import { HeaderComponent } from './header/header.component';
import { HeaderModule } from './header/header.module';
import { HomeModule } from './home-page/home-page.module';

// Definir as rotas da aplicação
const routes: Routes = [
  {
    path: '',
    component: HomePageComponent,
    pathMatch: 'full'
  }
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes),
  ],
  exports: [
    HeaderModule,
    FooterModule,
    HomeModule
  ]
})
export class AppRoutingModule { }
