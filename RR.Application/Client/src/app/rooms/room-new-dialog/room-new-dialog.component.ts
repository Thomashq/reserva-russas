
import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, FormsModule } from '@angular/forms';
import { Account } from '../../domain/models/account';
import { Subject } from 'rxjs';
import { takeUntil, finalize } from 'rxjs/operators';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Rooms } from '../../domain/models/rooms';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { CreateRoomRequest } from '../../domain/dto/request/RoomsRequest';
import { RoomsService } from '../rooms.service';
import { AuthService } from '../../auth/auth.service';

@Component({
  selector: 'app-room-new-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    FormsModule
  ],
  templateUrl: './room-new-dialog.component.html',
  styleUrl: './room-new-dialog.component.css'
})
export class RoomNewDialogComponent implements OnInit, OnDestroy {
  frmCreateRoom!: FormGroup;
  isLoading = false;

  account: Account | null = null;
  createdRoom: Rooms | null = null;
  roomRequest!: CreateRoomRequest;

  private destroy$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private roomService: RoomsService,
    private authService: AuthService,
    private snackbar: MatSnackBar,
    private dialogRef: MatDialogRef<RoomNewDialogComponent>
  ) { }

  ngOnInit(): void {
    this.authService.currentUser$
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (account) => this.account = account,
        error: () => this.account = null
      });

    this.frmCreateRoom = this.fb.group({
      Name: [null, Validators.required],
      Capacity: [null, [Validators.required, Validators.min(1)]],
      ManagerId: [2]
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  salvarSala(): void {
    if (this.account?.AccountPermission != 0 && this.account?.AccountPermission != 3) {
      this.snackbar.open('Você não tem permissão para criar salas.', 'Fechar', { duration: 5000 });
      this.dialogRef.close();
      return;
    }

    if (this.frmCreateRoom.invalid) {
      this.snackbar.open('Por favor, preencha todos os campos obrigatórios.', 'Fechar', { duration: 5000 });
      return;
    }

    this.isLoading = true;
    this.frmCreateRoom.disable();

    this.roomRequest = this.frmCreateRoom.getRawValue();

    this.roomService.AddAsync(this.roomRequest)
      .pipe(finalize(() => {
        this.isLoading = false;
        this.frmCreateRoom.enable();
      }))
      .subscribe({
        next: (room) => {
          this.createdRoom = room;
          this.snackbar.open('Sala criada com sucesso.', 'Fechar', { duration: 4000 });
          this.dialogRef.close(room);
        },
        error: (err) => {
          console.error(err);
          this.snackbar.open('Erro ao criar sala. Tente novamente.', 'Fechar', { duration: 5000 });
          this.dialogRef.close();
        }
      });
  }
}
