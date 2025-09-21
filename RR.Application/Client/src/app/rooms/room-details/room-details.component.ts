import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatListModule } from '@angular/material/list';
import { RoomsService } from '../rooms.service';
import { Router, ActivatedRoute } from '@angular/router';
import { DateOffset } from '../../domain/shared/utils/date-offset.util';

@Component({
  selector: 'app-room-details',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatListModule],
  templateUrl: './room-details.component.html',
  styleUrls: ['./room-details.component.css']
})
export class RoomDetailsComponent implements OnInit {
  constructor(
    private roomService: RoomsService,
    private router: Router,
    private route: ActivatedRoute
  ) { }

  roomId: number = 0;
  room: any = null;
  isLoading = false;

  // reservas que estão chegando (hoje -> +7 dias)
  upcomingReservations: any[] = [];

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.roomId = +params['id']; // Pega o id da rota
      if (!this.roomId) return;

      this.fetchRoomDetails();
      this.fetchUpcomingReservations();
    });
  }

  private fetchRoomDetails() {
    this.isLoading = true;
    this.roomService.GetById(this.roomId).subscribe({
      next: (result) => {
        this.room = result;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('There was an error!', error);
        this.isLoading = false;
      }
    });
  }

  // Semana inteira: hoje 00:00 -> +7 dias 23:59:59, no formato DateTimeOffset do C#
  fetchUpcomingReservations(): void {
    const { start, end } = DateOffset.weekWindow(); // já retorna "yyyy-MM-ddTHH:mm:ss±HH:mm"

    this.roomService.GetRoomsReservationsByPeriod(this.roomId, start, end).subscribe({
      next: (result) => {
        // Interceptor já padroniza — só atribuir
        this.upcomingReservations = result ?? [];
      },
      error: (err) => {
        console.error('Error loading upcoming reservations', err);
      }
    });
  }
}
