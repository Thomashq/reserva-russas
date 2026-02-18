import { Component, OnInit, OnDestroy, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Subject, takeUntil } from 'rxjs';
import { EquipmentReservationService } from '../equipment-reservation.service';
import { Equipment } from '../../domain/models/equipment';
import { CreateEquipmentReservationRequest } from '../../domain/dto/request/EquipmentReservationRequest';
import { EquipmentService } from '../equipment-service';

@Component({
  selector: 'app-equipment-reservation-new-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatSnackBarModule
  ],
  templateUrl: './equipment-reservation-new-dialog.html',
  styleUrls: ['./equipment-reservation-new-dialog.css']
})
export class EquipmentReservationNewDialogComponent implements OnInit, OnDestroy {
  reservationForm: FormGroup;
  equipmentList: Equipment[] = [];
  isLoading = false;
  isSubmitting = false;

  private destroy$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<EquipmentReservationNewDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { accountId: number, equipmentId?: number },
    private equipmentReservationService: EquipmentReservationService,
    private equipmentService: EquipmentService,
    private snackbar: MatSnackBar
  ) {
    this.reservationForm = this.fb.group({
      equipmentId: [data?.equipmentId || null, Validators.required],
      title: ['', [Validators.required, Validators.maxLength(250)]],
      description: ['', Validators.maxLength(1000)],
      startDate: [null, Validators.required],
      startTime: ['', Validators.required],
      endDate: [null, Validators.required],
      endTime: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.loadEquipment();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadEquipment(): void {
    this.isLoading = true;
    this.equipmentService.GetAll()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (result) => {
          this.equipmentList = result?.filter(e => e.isActive) ?? [];
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Erro ao carregar equipamentos', error);
          this.snackbar.open('Erro ao carregar equipamentos.', 'Fechar', { duration: 5000 });
          this.isLoading = false;
        }
      });
  }

  onSubmit(): void {
    if (this.reservationForm.invalid) {
      this.snackbar.open('Por favor, preencha todos os campos obrigatórios.', 'Fechar', { duration: 4000 });
      return;
    }

    const formValue = this.reservationForm.value;

    // Combina data e hora
    const startDateTime = this.combineDateTime(formValue.startDate, formValue.startTime);
    const endDateTime = this.combineDateTime(formValue.endDate, formValue.endTime);

    if (startDateTime >= endDateTime) {
      this.snackbar.open('A data/hora de início deve ser anterior à data/hora de término.', 'Fechar', { duration: 4000 });
      return;
    }

    const request: CreateEquipmentReservationRequest = {
      equipmentId: formValue.equipmentId,
      accountId: this.data.accountId,
      title: formValue.title,
      description: formValue.description || undefined,
      startTime: startDateTime.toISOString(),
      endTime: endDateTime.toISOString()
    };

    this.isSubmitting = true;

    this.equipmentReservationService.Create(request)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (result) => {
          this.isSubmitting = false;
          this.dialogRef.close(result);
        },
        error: (error) => {
          console.error('Erro ao criar reserva de equipamento', error);
          this.isSubmitting = false;

          const errorMsg = error?.error?.message || error?.message || 'Erro ao criar reserva.';
          this.snackbar.open(errorMsg, 'Fechar', { duration: 5000 });
        }
      });
  }

  combineDateTime(date: Date, time: string): Date {
    const [hours, minutes] = time.split(':').map(Number);
    const result = new Date(date);
    result.setHours(hours, minutes, 0, 0);
    return result;
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
