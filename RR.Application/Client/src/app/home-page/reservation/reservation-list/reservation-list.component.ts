
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
import { DateOffset } from '../../../domain/shared/utils/date-offset.util';
import { ReservationNewDialogComponent } from '../reservation-new-dialog/reservation-new-dialog.component';
import { ReservationEditDialogComponent } from '../reservation-edit-dialog/reservation-edit-dialog';
import { AuthService } from '../../../auth/auth.service';
import { Account } from '../../../domain/models/account';
import { Subject, takeUntil, finalize } from 'rxjs';

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
  templateUrl: './reservation-list.component.html',
  styleUrls: ['./reservation-list.component.css']
})
export class ReservationListComponent implements OnInit, OnDestroy {
  isLoading = false;

  account: Account | null = null;
  accountId = 0;

  private destroy$ = new Subject<void>();

  reservations: Reservations[] = [];
  displayedColumns: string[] = ['title', 'room', 'start', 'end', 'actions'];

  dataSource = new MatTableDataSource<Reservations>([]);
  pageSize = 10;

  @ViewChild(MatPaginator) paginator!: MatPaginator;

  constructor(
    private reservationService: ReservationService,
    private dialog: MatDialog,
    private authService: AuthService,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.loadCurrentWeek();

    this.authService.currentUser$
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (account) => {
          this.account = account;
          this.accountId = account?.Id ?? 0;
        },
        error: () => {
          this.account = null;
          this.accountId = 0;
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

    if (this.paginator) {
      this.dataSource.paginator = this.paginator;
    }
  }

  loadCurrentWeek(): void {
    const { start, end } = DateOffset.weekWindow();
    const period: PeriodRequest = { start, end } as PeriodRequest;

    this.isLoading = true;
    this.reservationService.GetReservationsByPeriod(period).pipe(
      finalize(() => this.isLoading = false)
    ).subscribe({
      next: (data) => this.applyData(data ?? []),
      error: (err) => {
        console.error('Erro ao carregar reservas:', err);
        this.applyData([]);
        this.snackBar.open('Erro ao carregar reservas', 'Fechar', { duration: 4000 });
      }
    });
  }

  refresh(): void {
    this.loadCurrentWeek();
  }

  getId(r: any): number {
    return Number(r?.Id ?? r?.id ?? 0);
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
    const isLoggedIn = this.authService.isLoggedIn();
    if (!isLoggedIn) {
      this.snackBar.open('O usuário precisa estar autenticado', 'Fechar', { duration: 5000 });
      return;
    }

    const dialogRef = this.dialog.open(ReservationNewDialogComponent, {
      width: '800px',
      maxHeight: '90vh',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) this.loadCurrentWeek();
    });
  }

  cancelReservation(reservationId: number): void {
    if (!reservationId) return;

    const ok = window.confirm('Cancelar (deletar) esta reserva?');
    if (!ok) return;

    this.isLoading = true;

    this.reservationService.DeleteReservation(reservationId).pipe(
      finalize(() => this.isLoading = false)
    ).subscribe({
      next: () => {
        this.snackBar.open('Reserva cancelada com sucesso!', 'Fechar', { duration: 3000 });
        this.loadCurrentWeek();
      },
      error: (err) => {
        console.error(err);
        this.snackBar.open('Não foi possível deletar a reserva', 'Fechar', { duration: 5000 });
      }
    });
  }

  editReservation(reservationId: number): void {
    if (!reservationId) return;

    const isLoggedIn = this.authService.isLoggedIn();
    if (!isLoggedIn) {
      this.snackBar.open('O usuário precisa estar autenticado', 'Fechar', { duration: 5000 });
      return;
    }

    const dialogRef = this.dialog.open(ReservationEditDialogComponent, {
      width: '800px',
      maxHeight: '90vh',
      disableClose: true,
      data: { reservationId }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) this.loadCurrentWeek();
    });
  }
}

