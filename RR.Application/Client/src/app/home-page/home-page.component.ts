import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

import { MatToolbarModule } from '@angular/material/toolbar';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';

import { AuthService } from '../auth/auth.service';
import { ReservationListComponent } from '../home-page/reservation/reservation-list/reservation-list.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CommonModule, RouterModule,
    MatToolbarModule, MatCardModule, MatButtonModule,
    ReservationListComponent
  ],
  templateUrl: './home-page.component.html',
  styleUrls: ['./home-page.component.css']
})
export class HomePageComponent {
  constructor(private authService: AuthService) { }
}
