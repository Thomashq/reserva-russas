import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, FormsModule } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CommonModule } from '@angular/common';
import { MatDialogModule } from '@angular/material/dialog';
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
  account: Account | null = null;
  accountId: number = 0;
  roomList: Rooms[] = [];
  private destroy$ = new Subject<void>();

  constructor(
    private dialogRef: MatDialogRef<ReservationNewDialogComponent>,
    private reservationService: ReservationService,
    private reservationSeriesService: ReservationSeriesService,
    private fb: FormBuilder,
    private authService: AuthService,
    private snackBar: MatSnackBar,
    private roomService: RoomsService
  ) { }

  ngOnInit(): void {
    this.frmCreateReservation = this.fb.group({
      roomId: [null, Validators.required],
      //accountId: [null, Validators.required],
      title: ['', Validators.required],
      description: [''],
      startTime: ['', Validators.required],
      endTime: ['', Validators.required]
    });

    this.frmCreateReservationSeries = this.fb.group({
      //accountId: [null, Validators.required],
      defaultRoomId: [null, Validators.required],
      title: ['', Validators.required],
      description: [''],
      windowStart: [null, Validators.required],
      windowEnd: [null, Validators.required],
      recurrenceRule: ['', Validators.required],
      daysOfWeek: [[], Validators.required],
      timeStart: ['', Validators.required],
      timeEnd: ['', Validators.required],
      interval: [1]
    });

    this.authService.currentUser$
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (account) => {
          this.account = account;
        },
        error: (error) => {
          this.account = null;
        }
      });

    this.loadRooms();
  }

  onDateChange(which: 'start' | 'end', date: Date | null): void {
    if (which === 'start') this._startDate = date; else this._endDate = date;
    this.tryPatchDateTime(which);
  }

  onTimeChange(which: 'start' | 'end', hhmm: string): void {
    if (which === 'start') this.startTimeDisplay = hhmm; else this.endTimeDisplay = hhmm;
    this.tryPatchDateTime(which);
  }

  private tryPatchDateTime(which: 'start' | 'end'): void {
    const date = which === 'start' ? this._startDate : this._endDate;
    const hhmm = which === 'start' ? this.startTimeDisplay : this.endTimeDisplay;
    if (!date || !hhmm || !hhmm.includes(':')) return;

    const dt = DateOffset.fromDateAndTime(date, hhmm);
    const ctrl = which === 'start' ? 'startTime' : 'endTime';
    this.frmCreateReservation.patchValue({ [ctrl]: dt });
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
    console.log(this.account?.Id)
    this.accountId = this.account?.Id || 0;

    reservation.accountId = this.accountId;

    this.reservationService.CreateReservation(reservation).pipe(
      finalize(() => this.isLoading = false)
    ).subscribe({
      next: () => {
        this.snackBar.open('Reserva criada com sucesso!', 'Fechar', { duration: 3000 });
        this.dialogRef.close(true); 
      },
      error: (err) => {
        this.snackBar.open('Erro ao criar reserva: ' + err.message, 'Fechar', { duration: 5000 });
      }
    });
  }

  onReservationSeriesSubmit(): void {
    if (this.frmCreateReservationSeries.invalid) return;

    const v = this.frmCreateReservationSeries.value;
    if (Array.isArray(v.daysOfWeek)) {
      this.frmCreateReservationSeries.patchValue({
        daysOfWeek: (v.daysOfWeek as string[]).join(',')
      });
    }

    if (v.windowStart instanceof Date) {
      const ws = DateOffset.format(DateOffset.startOfDay(v.windowStart));
      this.frmCreateReservationSeries.patchValue({ windowStart: ws });
    }
    if (v.windowEnd instanceof Date) {
      const we = DateOffset.format(DateOffset.endOfDay(v.windowEnd));
      this.frmCreateReservationSeries.patchValue({ windowEnd: we });
    }

    this.isLoading = true;
    const reservationSeries: CreateSeriesRequest = this.frmCreateReservationSeries.value;
    this.accountId = this.account?.id || 0;

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
      }
    });
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }

  private loadRooms(): void {
    this.roomService.GetAll()
      .subscribe({
        next: (rooms) => {
          rooms.forEach(room => {
            this.roomList.push(room);
            console.log(this.roomList)
          })
        },
        error: (err) => {
          this.snackBar.open('Erro ao carregar salas: ' + err.message, 'Fechar', { duration: 5000 });
        }
    })
  }
}
