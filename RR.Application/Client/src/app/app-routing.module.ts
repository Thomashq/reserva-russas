import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomePageComponent } from './home-page/home-page.component';

// Definir as rotas da aplicação
const routes: Routes = [
  {
    path: '',
    component: HomePageComponent,
    pathMatch: 'full'
  },
  {
    path: 'auth',
    loadChildren: () => import('./auth/auth-routing.module').then(m => m.AuthRoutingModule)
  },
  // Adicione suas rotas protegidas aqui quando necessário
  // Exemplo:
  // {
  //   path: 'sua-rota-protegida',
  //   canActivate: [AuthGuard],
  //   component: SeuComponent
  // },
  {
    path: '**',
    redirectTo: '', // Redireciona para home em caso de rota não encontrada
    pathMatch: 'full'
  }
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes, {
      enableTracing: false, // Set to true for debugging
      scrollPositionRestoration: 'top'
    })
  ],
  exports: [RouterModule]
})
export class AppRoutingModule { }
