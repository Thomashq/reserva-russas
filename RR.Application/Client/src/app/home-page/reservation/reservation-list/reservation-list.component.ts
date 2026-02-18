import { CommonModule } from '@angular/common';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { RouterModule } from '@angular/router';

import { MatCardModule } from '@angular/material/card';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatMenuModule } from '@angular/material/menu';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';

import { ReservationService } from '../reservation.service';
import { Reservations } from '../../../domain/models/reservations';
import { PeriodRequest } from '../../../domain/dto/request/ReservationRequest';
import { ReservationNewDialogComponent } from '../reservation-new-dialog/reservation-new-dialog.component';
import { ReservationEditDialogComponent } from '../reservation-edit-dialog/reservation-edit-dialog';
import { AuthService } from '../../../auth/auth.service';
import { Account } from '../../../domain/models/account';
import { Subject, takeUntil, finalize } from 'rxjs';
import { RoomsService } from '../../../rooms/rooms.service';

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
    MatMenuModule,
    MatPaginatorModule
  ],
  templateUrl: './reservation-list.component.html'
})
export class ReservationListComponent implements OnInit, OnDestroy {

  isLoading = false;

  account: Account | null = null;
  isLoggedIn = false;

  private destroy$ = new Subject<void>();

  reservations: Reservations[] = [];
  displayedColumns: string[] = ['time', 'room', 'title', 'actions'];

  dataSource = new MatTableDataSource<Reservations>([]);
  pageSize = 5;

  selectedDate: Date = new Date();

  private roomCache = new Map<number, string>();

  @ViewChild(MatPaginator) paginator!: MatPaginator;

  constructor(
    private reservationService: ReservationService,
    private roomsService: RoomsService,
    private dialog: MatDialog,
    private authService: AuthService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadDay();

    this.authService.currentUser$
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (acc) => {
          this.account = acc;
          this.isLoggedIn = !!acc;
        },
        error: () => {
          this.account = null;
          this.isLoggedIn = false;
        }
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private applyData(data: Reservations[]): void {
    this.reservations = data ?? [];
    this.dataSource.data = this.reservations;

    this.reservations.forEach(r => {
      const roomId = r.RoomId ?? (r as any)?.roomId;
      if (roomId) this.getRoomNameById(roomId);
    });

    if (this.paginator) {
      this.dataSource.paginator = this.paginator;
    }
  }

  loadDay(): void {
    const start = new Date(this.selectedDate);
    start.setHours(0, 0, 0, 0);

    const end = new Date(this.selectedDate);
    end.setHours(23, 59, 59, 0);

    const period: PeriodRequest = {
      start: start.toISOString().slice(0, 19),
      end: end.toISOString().slice(0, 19)
    };

    this.isLoading = true;

    this.reservationService.GetReservationsByPeriod(period).pipe(
      finalize(() => this.isLoading = false)
    ).subscribe({
      next: (data) => this.applyData(data ?? []),
      error: () => {
        this.applyData([]);
        this.snackBar.open('Erro ao carregar reservas', 'Fechar', { duration: 4000 });
      }
    });
  }

  previousDay(): void {
    this.selectedDate.setDate(this.selectedDate.getDate() - 1);
    this.selectedDate = new Date(this.selectedDate);
    this.loadDay();
  }

  nextDay(): void {
    this.selectedDate.setDate(this.selectedDate.getDate() + 1);
    this.selectedDate = new Date(this.selectedDate);
    this.loadDay();
  }

  today(): void {
    this.selectedDate = new Date();
    this.loadDay();
  }

  getRoomNameById(roomId: number): string {
    if (!roomId) return '';

    if (this.roomCache.has(roomId)) {
      return this.roomCache.get(roomId)!;
    }

    this.roomsService.GetById(roomId).subscribe({
      next: (room) => {
        const name = room?.Name ?? `#${roomId}`;
        this.roomCache.set(roomId, name);
      },
      error: () => {
        this.roomCache.set(roomId, `#${roomId}`);
      }
    });

    return `#${roomId}`;
  }

  openNewReservationDialog(): void {
    if (!this.isLoggedIn) return;

    const dialogRef = this.dialog.open(ReservationNewDialogComponent, {
      width: '800px',
      maxHeight: '90vh',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) this.loadDay();
    });
  }

  editReservation(reservationId: number): void {
    if (!this.isLoggedIn || !reservationId) return;

    const dialogRef = this.dialog.open(ReservationEditDialogComponent, {
      width: '800px',
      maxHeight: '90vh',
      disableClose: true,
      data: { reservationId }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) this.loadDay();
    });
  }

  cancelReservation(reservationId: number): void {
    if (!this.isLoggedIn || !reservationId) return;

    const ok = window.confirm('Cancelar esta reserva?');
    if (!ok) return;

    this.isLoading = true;

    this.reservationService.DeleteReservation(reservationId).pipe(
      finalize(() => this.isLoading = false)
    ).subscribe({
      next: () => {
        this.snackBar.open('Reserva cancelada', 'Fechar', { duration: 3000 });
        this.loadDay();
      },
      error: () => {
        this.snackBar.open('Erro ao cancelar reserva', 'Fechar', { duration: 4000 });
      }
    });
  }

  getId(r: any): number {
    return Number(r?.Id ?? r?.id ?? 0);
  }

  formatTime(dateTime: string): string {
    if (!dateTime) return '';
    return new Date(dateTime).toLocaleTimeString('pt-BR', {
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  formatDateLabel(): string {
    return this.selectedDate.toLocaleDateString('pt-BR', {
      weekday: 'long',
      day: '2-digit',
      month: '2-digit'
    });
  }
}
