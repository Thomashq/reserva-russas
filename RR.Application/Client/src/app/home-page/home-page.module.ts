import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { HomePageComponent } from './home-page.component';
//import { StatsModule } from '../stats/stats.module';

@NgModule({
  declarations: [
    HomePageComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    //StatsModule // Importando o módulo de estatísticas usado na home
  ],
  exports: [
    HomePageComponent
  ]
})
export class HomeModule { }
