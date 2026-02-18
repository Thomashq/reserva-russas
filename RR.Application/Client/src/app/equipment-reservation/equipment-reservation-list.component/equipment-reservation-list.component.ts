import { CommonModule } from '@angular/common';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { MatTooltipModule } from '@angular/material/tooltip';
import {MatChipsModule} from '@angular/material/chips';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Subject, takeUntil, forkJoin, map } from 'rxjs';
import { EquipmentReservationService } from '../equipment-reservation.service';
import { RoomsService } from '../../rooms/rooms.service';
import { AuthService } from '../../auth/auth.service';
import { Account } from '../../domain/models/account';
import { EquipmentReservation, Equipment } from '../../domain/models/equipment';
import { Rooms } from '../../domain/models/rooms';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { EquipmentService } from '../equipment-service';
import { EquipmentReservationNewDialogComponent } from '../equipment-reservation-new-dialog/equipment-reservation-new-dialog';

@Component({
  selector: 'app-equipment-reservation-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatButtonModule,
    MatCardModule,
    MatTableModule,
    MatIconModule,
    MatSnackBarModule,
    MatTooltipModule,
    MatSelectModule,
    MatFormFieldModule,
    MatChipsModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './equipment-reservation-list.component.html',
  styleUrls: ['./equipment-reservation-list.component.css']
})
export class EquipmentReservationListComponent implements OnInit, OnDestroy {
  isLoading = false;
  reservations: EquipmentReservation[] = [];
  filteredReservations: EquipmentReservation[] = [];
  roomsList: Rooms[] = [];
  equipmentList: Equipment[] = [];

  displayedColumns: string[] = ['equipment', 'title', 'period', 'status', 'actions'];

  account: Account | null = null;
  accountId = 0;
  isManagerOrAbove = false;

  // Filtros
  selectedRoomId: number | null = null;
  selectedEquipmentId: number | null = null;
  selectedStatus: number | null = null;

  private destroy$ = new Subject<void>();

  constructor(
    private equipmentReservationService: EquipmentReservationService,
    private equipmentService: EquipmentService,
    private roomsService: RoomsService,
    private router: Router,
    private authService: AuthService,
    private dialog: MatDialog,
    private snackbar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.authService.currentUser$
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (result) => {
          this.account = result;
          this.accountId = (result as any)?.Id ?? (result as any)?.id ?? 0;
          this.isManagerOrAbove = this.getAccountPermissionLevel(this.account);

          // Ajusta colunas baseado na permissão
          if (this.isManagerOrAbove && !this.displayedColumns.includes('account')) {
            this.displayedColumns.splice(3, 0, 'account');
          }

          // Carrega dados após obter o usuário
          if (this.account) {
            this.loadAllData();
          }
        },
        error: () => {
          this.account = null;
          this.accountId = 0;
          this.isManagerOrAbove = false;
        }
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadAllData(): void {
    this.isLoading = true;

    forkJoin({
      reservations: this.isManagerOrAbove
        ? this.equipmentReservationService.GetAll()
        : this.equipmentReservationService.GetByAccount(this.accountId),
      rooms: this.roomsService.GetAll(),
      equipment: this.equipmentService.GetAll()
    })
    .pipe(takeUntil(this.destroy$))
    .subscribe({
      next: (result) => {
        this.reservations = result.reservations ?? [];
        this.roomsList = result.rooms ?? [];
        this.equipmentList = result.equipment ?? [];
        this.applyFilters();
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Erro ao carregar dados', error);
        this.isLoading = false;
        this.snackbar.open('Erro ao carregar dados.', 'Fechar', { duration: 5000 });
      }
    });
  }

  onRoomFilterChange(): void {
    if (this.selectedRoomId) {
      this.roomsService.GetById(this.selectedRoomId)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: (room) => {
            if (room?.RoomDetails?.RoomEquipments) {
              const equipmentIds = room.RoomDetails.RoomEquipments.map(re => re.EquipmentId);
              this.equipmentList = this.equipmentList.filter(e => equipmentIds.includes(e.Id));
            }
            this.applyFilters();
          },
          error: (error) => {
            console.error('Erro ao carregar sala', error);
            this.applyFilters();
          }
        });
    } else {
      // Recarrega todos os equipamentos
      this.equipmentService.GetAll()
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: (equipment) => {
            this.equipmentList = equipment ?? [];
            this.applyFilters();
          }
        });
    }
  }

  onEquipmentFilterChange(): void {
    this.applyFilters();
  }

  applyFilters(): void {
    this.filteredReservations = this.reservations.filter(reservation => {
      let matches = true;

      // Filtro por equipamento
      if (this.selectedEquipmentId !== null) {
        matches = matches && reservation.EquipmentId === this.selectedEquipmentId;
      }

      // Filtro por status
      if (this.selectedStatus !== null) {
        matches = matches && reservation.Status === this.selectedStatus;
      }

      return matches;
    });
  }

  clearFilters(): void {
    this.selectedRoomId = null;
    this.selectedEquipmentId = null;
    this.selectedStatus = null;

    this.equipmentService.GetAll()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (equipment) => {
          this.equipmentList = equipment ?? [];
          this.applyFilters();
        }
      });
  }

  hasActiveFilters(): boolean {
    return this.selectedRoomId !== null ||
           this.selectedEquipmentId !== null ||
           this.selectedStatus !== null;
  }

  getAccountPermissionLevel(acc: Account | null): boolean {
    if (!acc) return false;
    const permission = (acc as any).permission ?? (acc as any).AccountPermission;
    return permission === 0 || permission === 3;
  }

  getStatusText(status: number): string {
    switch (status) {
      case 0: return 'Pendente';
      case 1: return 'Aprovado';
      case 2: return 'Cancelado';
      default: return 'Desconhecido';
    }
  }

  getStatusColor(status: number): string {
    switch (status) {
      case 0: return 'warn';
      case 1: return 'primary';
      case 2: return 'accent';
      default: return '';
    }
  }

  getStatusCount(status: number): number {
    return this.filteredReservations.filter(r => r.Status === status).length;
  }

  formatPeriod(reservation: EquipmentReservation): string {
    const start = new Date(reservation.StartTime);
    const end = new Date(reservation.EndTime);
    const dateFormat = start.toLocaleDateString('pt-BR', { day: '2-digit', month: '2-digit' });
    const startTime = start.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' });
    const endTime = end.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' });
    return `${dateFormat} ${startTime} - ${endTime}`;
  }

  getAccountName(reservation: EquipmentReservation): string {
    return (reservation.Account as any)?.name ||
           (reservation.Account as any)?.Name ||
           'N/A';
  }

  openNewReservationDialog(): void {
    if (!this.account) {
      this.snackbar.open('Você precisa estar logado para reservar equipamentos.', 'Fechar', { duration: 5000 });
      return;
    }

    const dialogRef = this.dialog.open(EquipmentReservationNewDialogComponent, {
      width: '800px',
      maxHeight: '90vh',
      disableClose: true,
      data: {
        accountId: this.accountId,
        equipmentId: this.selectedEquipmentId
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.snackbar.open('Reserva de equipamento criada com sucesso.', 'Fechar', { duration: 4000 });
        this.loadAllData();
      }
    });
  }

  cancelReservation(reservation: EquipmentReservation): void {
    if (!confirm(`Tem certeza que deseja cancelar a reserva "${reservation.Title}"?`)) {
      return;
    }

    this.equipmentReservationService.Cancel(reservation.Id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.snackbar.open('Reserva cancelada com sucesso.', 'Fechar', { duration: 3000 });
          this.loadAllData();
        },
        error: (error) => {
          console.error('Erro ao cancelar reserva', error);
          this.snackbar.open('Erro ao cancelar reserva.', 'Fechar', { duration: 5000 });
        }
      });
  }

  approveReservation(reservation: EquipmentReservation): void {
    this.equipmentReservationService.Approve(reservation.Id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.snackbar.open('Reserva aprovada com sucesso.', 'Fechar', { duration: 3000 });
          this.loadAllData();
        },
        error: (error) => {
          console.error('Erro ao aprovar reserva', error);
          this.snackbar.open('Erro ao aprovar reserva.', 'Fechar', { duration: 5000 });
        }
      });
  }

  deleteReservation(reservation: EquipmentReservation): void {
    if (!confirm(`Tem certeza que deseja excluir a reserva "${reservation.Title}"?`)) {
      return;
    }

    this.equipmentReservationService.Delete(reservation.Id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.snackbar.open('Reserva excluída com sucesso.', 'Fechar', { duration: 3000 });
          this.loadAllData();
        },
        error: (error) => {
          console.error('Erro ao excluir reserva', error);
          this.snackbar.open('Erro ao excluir reserva.', 'Fechar', { duration: 5000 });
        }
      });
  }

  viewDetails(reservation: EquipmentReservation): void {
    // Implementar modal de detalhes ou navegação
    console.log('Ver detalhes:', reservation);
    this.snackbar.open('Funcionalidade de detalhes em desenvolvimento.', 'Fechar', { duration: 3000 });
  }

  canApprove(): boolean {
    return this.isManagerOrAbove;
  }
}
