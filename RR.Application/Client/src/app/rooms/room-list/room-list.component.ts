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

@Component({
  selector: 'app-room-list',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatCardModule, MatTableModule, MatSnackBarModule],
  templateUrl: './room-list.component.html',
  styleUrls: ['./room-list.component.css']
})
export class RoomListComponent implements OnInit {
  constructor(
    private rommService: RoomsService,
    private router: Router,
    private authService: AuthService,
    private dialog: MatDialog,
    private snackbar: MatSnackBar
  ) { }

  isLoading = false;
  rooms: Rooms[] = [];

  // IDs DEVEM bater com os matColumnDef do template
  displayedColumns: string[] = ['name', 'capacity', 'actions'];

  ngOnInit(): void {
    this.isLoading = true;
    this.rommService.GetAll().subscribe({
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
    // tolera API retornando Id/id
    return (room as any).id ?? (room as any).Id;
  }

  openRoom(room: Rooms | number) {
    const id = typeof room === 'number' ? room : this.getId(room);
    if (id == null) {
      this.snackbar.open('ID da sala inválido.', 'Fechar', { duration: 4000 });
      return;
    }
    this.router.navigate(['/rooms', id]);
  }

  openNewRoomDialog() {
    if (this.authService.isLoggedIn()) {
      const dialogRef = this.dialog.open(RoomNewDialogComponent, {
        width: '800px',
        maxHeight: '90vh',
        disableClose: true // impede fechar clicando fora enquanto salva
      });

      dialogRef.afterClosed().subscribe(result => {
        if (result) {
          // result pode ser a sala criada ou apenas o id
          const created = result as any;
          const newId = created?.id ?? created?.Id ?? result;

          // Atualiza a lista local (se veio a entidade completa)
          if (created?.name || created?.Name) {
            this.rooms = [created, ...this.rooms];
          } else if (Number.isFinite(newId)) {
            // opcional: buscar o item criado e inserir na lista
            this.rommService.GetById(newId).subscribe(r => {
              if (r) this.rooms = [r, ...this.rooms];
            });
          }

          this.snackbar.open('Sala criada com sucesso.', 'Fechar', { duration: 4000 });
          if (Number.isFinite(newId)) {
            this.openRoom(newId);
          }
        }
      });
    } else {
      this.snackbar.open('Você precisa estar logado para criar uma sala.', 'Fechar', { duration: 5000 });
    }
  }


  newReservation(room: Rooms) {
    const id = this.getId(room);
    if (id == null) {
      this.snackbar.open('ID da sala inválido.', 'Fechar', { duration: 4000 });
      return;
    }
    this.router.navigate(['/reservations/new', { roomId: id }]);
  }
}
