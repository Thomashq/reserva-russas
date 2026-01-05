import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, FormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CommonModule } from '@angular/common';

import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';

import { finalize, Subject, takeUntil } from 'rxjs';
import { ReservationService } from '../reservation.service';
import { AuthService } from '../../../auth/auth.service';
import { RoomsService } from '../../../rooms/rooms.service';
import { Rooms } from '../../../domain/models/rooms';
import { Account } from '../../../domain/models/account';
import { DateOffset } from '../../../domain/shared/utils/date-offset.util';
import { UpdateReservationRequest } from '../../../domain/dto/request/ReservationRequest';

@Component({
  selector: 'app-reservation-edit-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatDatepickerModule,
    MatSelectModule,
    MatProgressSpinnerModule,
    MatIconModule,
    FormsModule
  ],
  templateUrl: './reservation-edit-dialog.html',
  styleUrls: ['./reservation-edit-dialog.css']
})
export class ReservationEditDialogComponent implements OnInit {
  frm!: FormGroup;
  isLoading = false;

  private destroy$ = new Subject<void>();

  account: Account | null = null;
  accountId = 0;

  roomList: Rooms[] = [];

  startTimeDisplay = '';
  endTimeDisplay = '';
  private _endTimeTouched = false;

  isReadOnly = false;

  constructor(
    private dialogRef: MatDialogRef<ReservationEditDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { reservationId: number, readOnly?: boolean },
    private reservationService: ReservationService,
    private fb: FormBuilder,
    private authService: AuthService,
    private snackBar: MatSnackBar,
    private roomService: RoomsService
  ) { }

  ngOnInit(): void {
    this.isReadOnly = !!this.data?.readOnly;

    this.frm = this.fb.group({
      roomId: [null, Validators.required],
      title: ['', Validators.required],
      description: [''],
      date: [null, Validators.required],
      startTime: ['', Validators.required],
      endTime: ['', Validators.required]
    });

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

    this.loadRooms();
    this.loadReservation();
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }

  enableEdit(): void {
    this.isReadOnly = false;
    this.frm.enable();
  }

  onDateChange(date: Date | null): void {
    if (this.isReadOnly) return;

    this.frm.patchValue({ date });

    this.tryPatchDateTime('start');
    this.tryPatchDateTime('end');
  }

  onTimeChange(which: 'start' | 'end', hhmm: string): void {
    if (this.isReadOnly) return;

    if (which === 'start') {
      this.startTimeDisplay = hhmm;

      if (!this._endTimeTouched) {
        this.endTimeDisplay = this.addOneHour(hhmm);
      }

      this.tryPatchDateTime('start');
      if (!this._endTimeTouched) this.tryPatchDateTime('end');
      return;
    }

    this._endTimeTouched = true;
    this.endTimeDisplay = hhmm;
    this.tryPatchDateTime('end');
  }

  private tryPatchDateTime(which: 'start' | 'end'): void {
    const date: Date | null = this.frm.value.date ?? null;
    const hhmm = which === 'start' ? this.startTimeDisplay : this.endTimeDisplay;
    if (!date || !hhmm || !hhmm.includes(':')) return;

    const dt = DateOffset.fromDateAndTime(date, hhmm);
    const ctrl = which === 'start' ? 'startTime' : 'endTime';
    this.frm.patchValue({ [ctrl]: dt });
  }

  private addOneHour(hhmm: string): string {
    const parts = hhmm.split(':');
    if (parts.length < 2) return hhmm;

    const hh = Number(parts[0]);
    const mm = Number(parts[1]);
    if (Number.isNaN(hh) || Number.isNaN(mm)) return hhmm;

    const total = (hh * 60 + mm + 60) % (24 * 60);
    const nh = Math.floor(total / 60);
    const nm = total % 60;

    return `${String(nh).padStart(2, '0')}:${String(nm).padStart(2, '0')}`;
  }

  private loadRooms(): void {
    this.roomService.GetAll().subscribe({
      next: (rooms) => {
        this.roomList = rooms ?? [];
      },
      error: (err) => {
        this.snackBar.open('Erro ao carregar salas: ' + err.message, 'Fechar', { duration: 5000 });
      }
    });
  }

  private loadReservation(): void {
    const id = this.data?.reservationId ?? 0;
    if (!id) return;

    this.isLoading = true;

    this.reservationService.GetReservationById(id).pipe(
      finalize(() => this.isLoading = false)
    ).subscribe({
      next: (r: any) => {
        if (!r) return;

        const roomId = Number(r.RoomId ?? r.roomId ?? null);
        const title = r.Title ?? r.title ?? '';
        const description = r.Description ?? r.description ?? '';

        const startIso = r.StartTime ?? r.startTime ?? '';
        const endIso = r.EndTime ?? r.endTime ?? '';

        const s = startIso ? new Date(startIso) : null;
        const e = endIso ? new Date(endIso) : null;

        const dateOnly = s ? new Date(s) : null;
        if (dateOnly) dateOnly.setHours(0, 0, 0, 0);

        this.frm.patchValue({
          roomId,
          title,
          description,
          date: dateOnly,
          startTime: startIso,
          endTime: endIso
        });

        if (s) this.startTimeDisplay = String(s.toTimeString()).slice(0, 5);
        if (e) this.endTimeDisplay = String(e.toTimeString()).slice(0, 5);
        this._endTimeTouched = true;

        if (this.isReadOnly) this.frm.disable();
      },
      error: () => {
        this.snackBar.open('Erro ao carregar reserva', 'Fechar', { duration: 4000 });
        this.dialogRef.close(false);
      }
    });
  }

  onSubmit(): void {
    if (this.isReadOnly) return;
    if (this.frm.invalid) return;

    this.tryPatchDateTime('start');
    this.tryPatchDateTime('end');

    const startIso = this.frm.value.startTime;
    const endIso = this.frm.value.endTime;

    if (!startIso || !endIso || new Date(startIso) >= new Date(endIso)) {
      this.snackBar.open('Verifique data e hora (início < fim).', 'Fechar', { duration: 4000 });
      return;
    }

    const id = this.data?.reservationId ?? 0;
    if (!id) return;

    const payload: UpdateReservationRequest = {
      roomId: this.frm.value.roomId,
      title: this.frm.value.title,
      description: this.frm.value.description,
      startTime: startIso,
      endTime: endIso
    } as UpdateReservationRequest;

    (payload as any).accountId = this.accountId;

    this.isLoading = true;

    this.reservationService.UpdateReservation(id, payload).pipe(
      finalize(() => this.isLoading = false)
    ).subscribe({
      next: () => {
        this.snackBar.open('Reserva atualizada com sucesso!', 'Fechar', { duration: 3000 });
        this.dialogRef.close(true);
      },
      error: (err) => {
        this.snackBar.open('Erro ao atualizar reserva: ' + err.message, 'Fechar', { duration: 5000 });
        this.dialogRef.close(false);
      }
    });
  }
}

