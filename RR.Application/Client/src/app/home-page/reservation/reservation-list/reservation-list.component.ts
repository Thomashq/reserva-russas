import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatMenuModule } from '@angular/material/menu';
import { MatDialog } from '@angular/material/dialog';
import { ReservationService } from '../reservation.service';
import { Reservations } from '../../../domain/models/reservations';
import { PeriodRequest } from '../../../domain/dto/request/ReservationRequest';
import { DateOffset } from '../../../domain/shared/utils/date-offset.util';
import { ReservationNewDialogComponent } from '../reservation-new-dialog/reservation-new-dialog.component';
import { AuthService } from '../../../auth/auth.service'

@Component({
  selector: 'app-reservation-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatMenuModule
  ],
  templateUrl: './reservation-list.component.html',
  styleUrls: ['./reservation-list.component.css']
})
export class ReservationListComponent implements OnInit {
  isLoading = false;
  reservations: Reservations[] = [];
  displayedColumns = ['title', 'room', 'start', 'end', 'actions'];

  constructor(
    private reservationService: ReservationService,
    private dialog: MatDialog,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    this.loadCurrentWeek();
  }

  loadCurrentWeek(): void {
    const { start, end } = DateOffset.weekWindow();
    const period: PeriodRequest = { start, end } as PeriodRequest;

    this.isLoading = true;
    this.reservationService.GetReservationsByPeriod(period).subscribe({
      next: (data) => this.reservations = data ?? [],
      error: (err) => console.error('Erro ao carregar reservas:', err),
      complete: () => this.isLoading = false
    });
  }

  refresh(): void {
    this.loadCurrentWeek();
  }

  formatDateTime(dateTime: string): string {
    if (!dateTime) return 'N/A';

    const date = new Date(dateTime);
    return date.toLocaleString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  openNewReservationDialog(): void {
    const dialogRef = this.dialog.open(ReservationNewDialogComponent, {
      width: '800px',
      maxHeight: '90vh',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        // Refresh the list if a reservation was created
        this.loadCurrentWeek();
      }
    });
  }
}
