import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, FormsModule } from '@angular/forms';
import { MatDialogRef, MatDialogModule, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CommonModule } from '@angular/common';
import { MatTabsModule } from '@angular/material/tabs';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../../auth/auth.service';
import { CreateReservationRequest } from '../../../domain/dto/request/ReservationRequest';
import { ReservationService } from '../reservation.service';
import { finalize, Subject, takeUntil } from 'rxjs';
import { CreateSeriesRequest } from '../../../domain/dto/request/CreateSeriesRequest';
import { ReservationSeriesService } from '../reservation-series.service';
import { DateOffset } from '../../../domain/shared/utils/date-offset.util';
import { Account } from '../../../domain/models/account';
import { RoomsService } from '../../../rooms/rooms.service';
import { Rooms } from '../../../domain/models/rooms';

@Component({
  selector: 'app-reservation-new-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatTabsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatDatepickerModule,
    MatSelectModule,
    MatProgressSpinnerModule,
    FormsModule
  ],
  templateUrl: './reservation-new-dialog.component.html',
  styleUrls: ['./reservation-new-dialog.component.css']
})
export class ReservationNewDialogComponent implements OnInit {
  frmCreateReservation!: FormGroup;
  frmCreateReservationSeries!: FormGroup;
  isLoading = false;

  selectedTabIndex = 0;

  weekdays = [
    { label: 'Segunda-feira', value: 'MO' },
    { label: 'Terça-feira', value: 'TU' },
    { label: 'Quarta-feira', value: 'WE' },
    { label: 'Quinta-feira', value: 'TH' },
    { label: 'Sexta-feira', value: 'FR' },
    { label: 'Sábado', value: 'SA' },
    { label: 'Domingo', value: 'SU' },
  ];

  private _startDate: Date | null = null;
  private _endDate: Date | null = null;

  startTimeDisplay = '';
  endTimeDisplay = '';
  private _endTimeTouched = false;

  account: Account | null = null;
  accountId = 0;
  roomList: Rooms[] = [];
  private destroy$ = new Subject<void>();

  constructor(
    private dialogRef: MatDialogRef<ReservationNewDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { roomId?: number; tab?: number } | null,
    private reservationService: ReservationService,
    private reservationSeriesService: ReservationSeriesService,
    private fb: FormBuilder,
    private authService: AuthService,
    private snackBar: MatSnackBar,
    private roomService: RoomsService
  ) { }

  ngOnInit(): void {
    this.selectedTabIndex = Number(this.data?.tab ?? 0);

    this.frmCreateReservation = this.fb.group({
      roomId: [null, Validators.required],
      title: ['', Validators.required],
      description: [''],
      startTime: ['', Validators.required],
      endTime: ['', Validators.required]
    });

    this.frmCreateReservationSeries = this.fb.group({
      defaultRoomId: [null, Validators.required],
      title: ['', Validators.required],
      description: [''],
      windowStart: [null, Validators.required],
      windowEnd: [null, Validators.required],
      recurrenceRule: [{ value: 'WEEKLY', disabled: true }, Validators.required],
      daysOfWeek: [[], Validators.required],
      timeStart: ['', Validators.required],
      timeEnd: ['', Validators.required],
      interval: [{ value: 1, disabled: true }]
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

    const preRoomId = Number(this.data?.roomId ?? 0);
    if (preRoomId) {
      this.frmCreateReservation.patchValue({ roomId: preRoomId });
      this.frmCreateReservationSeries.patchValue({ defaultRoomId: preRoomId });
    }
  }

  onDateChange(which: 'start' | 'end', date: Date | null): void {
    if (which === 'start') this._startDate = date; else this._endDate = date;

    this._endDate = this._startDate;

    this.tryPatchDateTime('start');
    this.tryPatchDateTime('end');
  }

  onTimeChange(which: 'start' | 'end', hhmm: string): void {
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
    const date = which === 'start' ? this._startDate : this._endDate;
    const hhmm = which === 'start' ? this.startTimeDisplay : this.endTimeDisplay;
    if (!date || !hhmm || !hhmm.includes(':')) return;

    const dt = DateOffset.fromDateAndTime(date, hhmm);
    const ctrl = which === 'start' ? 'startTime' : 'endTime';
    this.frmCreateReservation.patchValue({ [ctrl]: dt });
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

    const sh = String(nh).padStart(2, '0');
    const sm = String(nm).padStart(2, '0');
    return `${sh}:${sm}`;
  }

  onReservationSubmit(): void {
    if (this.frmCreateReservation.invalid) return;

    const startIso = this.frmCreateReservation.value.startTime;
    const endIso = this.frmCreateReservation.value.endTime;
    if (!startIso || !endIso || new Date(startIso) >= new Date(endIso)) {
      this.snackBar.open('Verifique data e hora (início < fim).', 'Fechar', { duration: 4000 });
      return;
    }

    this.isLoading = true;
    const reservation: CreateReservationRequest = this.frmCreateReservation.value;

    this.accountId = this.account?.Id || 0;
    (reservation as any).accountId = this.accountId;

    this.reservationService.CreateReservation(reservation).pipe(
      finalize(() => this.isLoading = false)
    ).subscribe({
      next: () => {
        this.snackBar.open('Reserva criada com sucesso!', 'Fechar', { duration: 3000 });
        this.dialogRef.close(true);
      },
      error: (err) => {
        this.snackBar.open('Erro ao criar reserva: ' + err.message, 'Fechar', { duration: 5000 });
        this.dialogRef.close(false);
      }
    });
  }

  onReservationSeriesSubmit(): void {
    if (this.frmCreateReservationSeries.invalid) return;

    const raw = this.frmCreateReservationSeries.getRawValue();

    const days = Array.isArray(raw.daysOfWeek)
      ? raw.daysOfWeek
      : (typeof raw.daysOfWeek === 'string' && raw.daysOfWeek.length ? raw.daysOfWeek.split(',') : []);

    const ws = raw.windowStart instanceof Date ? raw.windowStart : null;
    const we = raw.windowEnd instanceof Date ? raw.windowEnd : null;

    if (!ws || !we) {
      this.snackBar.open('Verifique a janela de datas (início e fim).', 'Fechar', { duration: 4000 });
      return;
    }

    const reservationSeries: CreateSeriesRequest = {
      ...raw,
      recurrenceRule: 'WEEKLY',
      interval: 1,
      daysOfWeek: days.join(','),
      windowStart: DateOffset.format(DateOffset.startOfDay(ws)),
      windowEnd: DateOffset.format(DateOffset.endOfDay(we))
    };

    this.isLoading = true;

    reservationSeries.accountId = this.accountId;

    this.reservationSeriesService.CreateSeries(reservationSeries).pipe(
      finalize(() => this.isLoading = false)
    ).subscribe({
      next: () => {
        this.snackBar.open('Série de reservas criada com sucesso!', 'Fechar', { duration: 3000 });
        this.dialogRef.close(true);
      },
      error: (err) => {
        this.snackBar.open('Erro ao criar série: ' + err.message, 'Fechar', { duration: 5000 });
        this.dialogRef.close(false);
      }
    });
  }

  onCancel(): void {
    this.dialogRef.close(false);
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
}
