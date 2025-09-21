import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';

import { ReservationService } from '../reservation.service';
import { Reservations } from '../../../domain/models/reservations';
import { PeriodRequest } from '../../../domain/dto/request/ReservationRequest';
import { DateOffset } from '../../../domain/shared/utils/date-offset.util';

@Component({
  selector: 'app-reservation-list',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatTableModule, MatButtonModule],
  templateUrl: './reservation-list.component.html',
  styleUrls: ['./reservation-list.component.css']
})
export class ReservationListComponent implements OnInit {
  constructor(
    private reservationService: ReservationService
  ) { }

  isLoading = false;
  reservations: Reservations[] = [];

  displayedColumns = ['title', 'room', 'start', 'end'];

  ngOnInit(): void {
    this.loadCurrentWeek();
  }

  loadCurrentWeek(): void {
    const { start, end } = DateOffset.weekWindow(); // "yyyy-MM-ddTHH:mm:ss±HH:mm"
    const period: PeriodRequest = { start, end } as PeriodRequest; // tipagem já existente

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
}
