import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { MatCardModule } from '@angular/material/card';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';

import { Subject, takeUntil, finalize } from 'rxjs';

import { ReservationService } from '../home-page/reservation/reservation.service';
import { Reservations } from '../domain/models/reservations';
import { PeriodRequest } from '../domain/dto/request/ReservationRequest';
import { DateOffset } from '../domain/shared/utils/date-offset.util';
import { AuthService } from '../auth/auth.service';
import { Account } from '../domain/models/account';
import { RoomsService } from '../rooms/rooms.service';
import { Rooms } from '../domain/models/rooms';
import { ReservationEditDialogComponent } from '../home-page/reservation/reservation-edit-dialog/reservation-edit-dialog';
import { ReservationNewDialogComponent } from '../home-page/reservation/reservation-new-dialog/reservation-new-dialog.component';

@Component({
  selector: 'app-reservation-page',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatCardModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatCheckboxModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatSelectModule,
    MatPaginatorModule
  ],
  templateUrl: './reservation-page.html',
  styleUrls: ['./reservation-page.css']
})
export class ReservationPageComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  isLoading = false;

  account: Account | null = null;
  accountId = 0;

  rooms: Rooms[] = [];

  // filtros (aplicam ao clicar Pesquisar)
  filterOnlyMine = false;
  filterRoomId: number | null = null;

  startDate!: Date;
  endDate!: Date;

  searchText = '';

  displayedColumns = ['title', 'room', 'start', 'end', 'status', 'actions'];

  dataSource = new MatTableDataSource<Reservations>([]);
  pageSize = 10;

  private roomCache = new Map<number, string>();

  @ViewChild(MatPaginator) paginator!: MatPaginator;

  constructor(
    private reservationService: ReservationService,
    private authService: AuthService,
    private roomService: RoomsService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    // últimos 15 dias usando DateOffset
    const today0 = DateOffset.startOfDay(new Date());
    this.endDate = DateOffset.endOfDay(today0);
    this.startDate = DateOffset.addDays(this.endDate, -15);

    this.authService.currentUser$
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (acc) => {
          this.account = acc;
          this.accountId = acc?.Id ?? 0;
        },
        error: () => {
          this.account = null;
          this.accountId = 0;
        }
      });

    this.loadRooms();
    this.search(); // primeira query
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private applyData(list: Reservations[]): void {
    this.dataSource.data = list ?? [];

    // Buscar nomes das salas
    list.forEach(r => {
      const roomId = r.RoomId;
      if (roomId) this.getRoomNameById(roomId);
    });

    if (this.paginator) {
      this.dataSource.paginator = this.paginator;
      this.paginator.firstPage();
    }
  }

  getRoomNameById(roomId: number): string {
    if (!roomId) return '';

    if (this.roomCache.has(roomId)) {
      return this.roomCache.get(roomId)!;
    }

    this.roomService.GetById(roomId).subscribe({
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

  loadRooms(): void {
    this.roomService.GetAll().subscribe({
      next: r => this.rooms = r ?? [],
      error: () => this.rooms = []
    });
  }

  search(): void {
    const period: PeriodRequest = {
      start: DateOffset.format(DateOffset.startOfDay(this.startDate)),
      end: DateOffset.format(DateOffset.endOfDay(this.endDate))
    } as PeriodRequest;

    this.isLoading = true;

    this.reservationService.GetReservationsByPeriod(period).pipe(
      finalize(() => this.isLoading = false)
    ).subscribe({
      next: (data) => {
        let result = data ?? [];

        if (this.filterOnlyMine && this.accountId) {
          result = result.filter(r => r.AccountId === this.accountId);
        }

        if (this.filterRoomId) {
          result = result.filter(r => r.RoomId === this.filterRoomId);
        }

        const q = (this.searchText ?? '').trim().toLowerCase();
        if (q) {
          result = result.filter(r => {
            const t = (r.Title ?? '').toLowerCase();
            const d = (r.Description ?? '').toLowerCase();
            const room = String(r.RoomId ?? '');
            return t.includes(q) || d.includes(q) || room.includes(q);
          });
        }

        this.applyData(result);
      },
      error: () => {
        this.applyData([]);
        this.snackBar.open('Erro ao carregar reservas', 'Fechar', { duration: 4000 });
      }
    });
  }

  // Enter ou submit do form
  onSearchEnter(): void {
    this.search();
  }

  openDetails(r: Reservations): void {
    const ref = this.dialog.open(ReservationEditDialogComponent, {
      width: '900px',
      maxHeight: '90vh',
      disableClose: true,
      data: { reservationId: r.Id, readOnly: true }
    });

    ref.afterClosed().subscribe(ok => {
      if (ok) this.search();
    });
  }

  editReservation(id: number): void {
    const ref = this.dialog.open(ReservationEditDialogComponent, {
      width: '900px',
      maxHeight: '90vh',
      disableClose: true,
      data: { reservationId: id, readOnly: false }
    });

    ref.afterClosed().subscribe(ok => {
      if (ok) this.search();
    });
  }

  cancelReservation(id: number): void {
    const ok = window.confirm('Cancelar esta reserva?');
    if (!ok) return;

    this.reservationService.DeleteReservation(id).subscribe({
      next: () => {
        this.snackBar.open('Reserva cancelada', 'Fechar', { duration: 3000 });
        this.search();
      },
      error: () => {
        this.snackBar.open('Erro ao cancelar reserva', 'Fechar', { duration: 4000 });
      }
    });
  }

  statusLabel(status?: number): string {
    switch (status) {
      case 0: return 'Criada';
      case 1: return 'Aprovada';
      case 2: return 'Cancelada';
      default: return '-';
    }
  }

  formatDateTime(dt: string): string {
    return new Date(dt).toLocaleString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  openNewReservation(): void {
    const ref = this.dialog.open(ReservationNewDialogComponent, {
      width: '900px',
      maxHeight: '90vh',
      disableClose: true
    });

    ref.afterClosed().subscribe(ok => {
      if (ok) this.search();
    });
  }
}
