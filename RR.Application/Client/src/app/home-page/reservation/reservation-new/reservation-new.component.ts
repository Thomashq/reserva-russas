import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { AuthService } from '../../../auth/auth.service';
import { CreateReservationRequest } from '../../../domain/dto/request/ReservationRequest';
import { ReservationService } from '../reservation.service';
import { finalize } from 'rxjs';
import { CreateSeriesRequest } from '../../../domain/dto/request/CreateSeriesRequest';
import { ReservationSeriesService } from '../reservation-series.service';
import { DateOffset } from '../../../domain/shared/utils/date-offset.util';

@Component({
  selector: 'app-reservation-new',
  imports: [ReactiveFormsModule],
  templateUrl: './reservation-new.component.html',
  styleUrls: ['./reservation-new.component.css']
})
export class ReservationNewComponent implements OnInit {
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

  // estado local para combinar data (datepicker) + hora (time picker)
  private _startDate: Date | null = null;
  private _endDate: Date | null = null;
  startTimeDisplay = '';
  endTimeDisplay = '';

  constructor(
    private reservationService: ReservationService,
    private reservationSeriesService: ReservationSeriesService,
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.frmCreateReservation = this.fb.group({
      roomId: [null, Validators.required],
      accountId: [null, Validators.required],
      title: ['', Validators.required],
      description: [''],
      startTime: ['', Validators.required], // será patchado com "yyyy-MM-ddTHH:mm:ss±HH:mm"
      endTime: ['', Validators.required]    // idem
    });

    this.frmCreateReservationSeries = this.fb.group({
      accountId: [null, Validators.required],
      defaultRoomId: [null, Validators.required],
      title: ['', Validators.required],
      description: [''],
      windowStart: [null, Validators.required], // será patchado p/ "yyyy-MM-ddTHH:mm:ss±HH:mm"
      windowEnd: [null, Validators.required],   // idem
      recurrenceRule: ['', Validators.required], // "WEEKLY" | "MONTHLY" | "YEARLY"
      daysOfWeek: [[], Validators.required],     // array de siglas; vira "MO,TU"
      timeStart: ['', Validators.required],      // "HH:mm"
      timeEnd: ['', Validators.required],        // "HH:mm"
      interval: [1]
    });
  }

  // === handlers chamados pelo template ===
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

    const dt = DateOffset.fromDateAndTime(date, hhmm); // "yyyy-MM-ddTHH:mm:ss±HH:mm"
    const ctrl = which === 'start' ? 'startTime' : 'endTime';
    this.frmCreateReservation.patchValue({ [ctrl]: dt });
  }

  onReservationSubmit(): void {
    if (this.frmCreateReservation.invalid) return;

    // validação simples: início < fim
    const startIso = this.frmCreateReservation.value.startTime;
    const endIso = this.frmCreateReservation.value.endTime;
    if (!startIso || !endIso || new Date(startIso) >= new Date(endIso)) {
      this.snackBar.open('Verifique data e hora (início < fim).', 'Fechar', { duration: 4000 });
      return;
    }

    this.isLoading = true;
    const reservation: CreateReservationRequest = this.frmCreateReservation.value;
    this.reservationService.CreateReservation(reservation).pipe(
      finalize(() => this.isLoading = false)
    ).subscribe({
      next: () => {
        this.snackBar.open('Reserva criada com sucesso!', 'Fechar', { duration: 3000 });
        // this.router.navigate(['/reservations', res.id]);
      },
      error: (err) => {
        this.snackBar.open('Erro ao criar reserva: ' + err.message, 'Fechar', { duration: 5000 });
      }
    });
  }

  onReservationSeriesSubmit(): void {
    if (this.frmCreateReservationSeries.invalid) return;

    // dias: ["MO","WE"] -> "MO,WE"
    const v = this.frmCreateReservationSeries.value;
    if (Array.isArray(v.daysOfWeek)) {
      this.frmCreateReservationSeries.patchValue({
        daysOfWeek: (v.daysOfWeek as string[]).join(',')
      });
    }

    // windowStart/windowEnd: garante offset no início/fim do dia
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
    this.reservationSeriesService.CreateSeries(reservationSeries).pipe(
      finalize(() => this.isLoading = false)
    ).subscribe({
      next: () => {
        this.snackBar.open('Série de reservas criada com sucesso!', 'Fechar', { duration: 3000 });
        // this.router.navigate(['/reservations', res.id]);
      }
    });
  }
}
