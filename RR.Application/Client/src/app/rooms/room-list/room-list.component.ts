import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { RoomsService } from '../rooms.service';
import { Router } from '@angular/router';
import { AuthService } from '../../auth/auth.service';
import { MatDialog } from '@angular/material/dialog';
import { RoomNewDialogComponent } from '../room-new-dialog/room-new-dialog.component';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Rooms } from '../../domain/models/rooms';

import { ReservationNewDialogComponent } from '../../home-page/reservation/reservation-new-dialog/reservation-new-dialog.component';

@Component({
  selector: 'app-room-list',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatCardModule, MatTableModule, MatSnackBarModule],
  templateUrl: './room-list.component.html',
  styleUrls: ['./room-list.component.css']
})
export class RoomListComponent implements OnInit {
  constructor(
    private roomService: RoomsService,
    private router: Router,
    private authService: AuthService,
    private dialog: MatDialog,
    private snackbar: MatSnackBar
  ) { }

  isLoading = false;
  rooms: Rooms[] = [];

  displayedColumns: string[] = ['name', 'capacity', 'actions'];

  ngOnInit(): void {
    this.isLoading = true;
    this.roomService.GetAll().subscribe({
      next: (result) => {
        this.rooms = result ?? [];
        this.isLoading = false;
      },
      error: (error) => {
        console.error('There was an error!', error);
        this.isLoading = false;
        this.snackbar.open('Erro ao carregar salas.', 'Fechar', { duration: 5000 });
      }
    });
  }

  private getId(room: Rooms): number {
    return (room as any).id ?? (room as any).Id;
  }

  openRoom(room: Rooms | number) {
    const id = typeof room === 'number' ? room : this.getId(room);
    if (!id) {
      this.snackbar.open('ID da sala inválido.', 'Fechar', { duration: 4000 });
      return;
    }
    this.router.navigate(['/room/detail', id]);
  }

  openNewRoomDialog() {
    if (!this.authService.isLoggedIn()) {
      this.snackbar.open('Você precisa estar logado para criar uma sala.', 'Fechar', { duration: 5000 });
      return;
    }

    const dialogRef = this.dialog.open(RoomNewDialogComponent, {
      width: '800px',
      maxHeight: '90vh',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      if (!result) return;

      const created = result as any;
      const newId = created?.id ?? created?.Id ?? result;

      if (created?.name || created?.Name) {
        this.rooms = [created, ...this.rooms];
      } else if (Number.isFinite(newId)) {
        this.roomService.GetById(newId).subscribe(r => {
          if (r) this.rooms = [r, ...this.rooms];
        });
      }

      this.snackbar.open('Sala criada com sucesso.', 'Fechar', { duration: 4000 });
      if (Number.isFinite(newId)) this.openRoom(newId);
    });
  }

  newReservation(room: Rooms) {
    if (!this.authService.isLoggedIn()) {
      this.snackbar.open('Você precisa estar logado para reservar.', 'Fechar', { duration: 5000 });
      return;
    }

    const id = this.getId(room);
    if (!id) {
      this.snackbar.open('ID da sala inválido.', 'Fechar', { duration: 4000 });
      return;
    }

    const dialogRef = this.dialog.open(ReservationNewDialogComponent, {
      width: '800px',
      maxHeight: '90vh',
      disableClose: true,
      data: { roomId: id, tab: 0 }
    });

    dialogRef.afterClosed().subscribe(ok => {
      if (ok) this.snackbar.open('Reserva criada.', 'Fechar', { duration: 3000 });
    });
  }
}
