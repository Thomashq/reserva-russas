//TODO: ajustar a lista de reservas nos próximos 7 dias, o período está bugando
import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatListModule } from '@angular/material/list';
import { RoomsService } from '../rooms.service';
import { Router, ActivatedRoute } from '@angular/router';
import { DateOffset } from '../../domain/shared/utils/date-offset.util';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';

import { ReservationNewDialogComponent } from '../../home-page/reservation/reservation-new-dialog/reservation-new-dialog.component';

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
    private route: ActivatedRoute,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) { }

  roomId: number = 0;
  room: any = null;
  isLoading = false;

  upcomingReservations: any[] = [];

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.roomId = +params['id'];
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

  fetchUpcomingReservations(): void {
    const { start, end } = DateOffset.weekWindow();

    this.roomService.GetRoomsReservationsByPeriod(this.roomId, start, end).subscribe({
      next: (result) => {
        this.upcomingReservations = result ?? [];
      },
      error: (err) => {
        console.error('Error loading upcoming reservations', err);
      }
    });
  }

  newReservation(): void {
    if (!this.roomId) return;

    const dialogRef = this.dialog.open(ReservationNewDialogComponent, {
      width: '800px',
      maxHeight: '90vh',
      disableClose: true,
      data: { roomId: this.roomId, tab: 0 }
    });

    dialogRef.afterClosed().subscribe(ok => {
      if (ok) {
        this.snackBar.open('Reserva criada.', 'Fechar', { duration: 3000 });
        this.fetchUpcomingReservations();
      }
    });
  }
}
