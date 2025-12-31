import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import {
  FormBuilder, FormGroup, Validators, AbstractControl, ValidationErrors,
  ReactiveFormsModule, FormControl, ValidatorFn
} from '@angular/forms';

import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';

import { AuthService } from '../auth.service';
import { RegisterRequest } from '../../domain/dto/request/RegisterRequest';
import { AccountCreatedResponse } from '../../domain/dto/response/AccountResponse';

interface RegisterFormControls {
  fullName: FormControl<string>;
  userName: FormControl<string>;
  email: FormControl<string>;
  password: FormControl<string>;
  confirmPassword: FormControl<string>;
  phone: FormControl<string | null>;
}

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule, RouterModule, ReactiveFormsModule,
    MatCardModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatIconModule, MatDividerModule
  ],
  templateUrl: './auth-register.component.html',
  styleUrls: ['./auth-register.component.css']
})
export class RegisterComponent {
  registerForm: FormGroup<RegisterFormControls>;
  errorMessage: string | null = null;
  isLoading = false;
  hidePassword = true;
  hideConfirm = true;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.registerForm = this.fb.group<RegisterFormControls>({
      fullName: this.fb.nonNullable.control('', [
        Validators.required,
        Validators.minLength(10),
        this.fullNameValidator()
      ]),
      userName: this.fb.nonNullable.control('', [
        Validators.required,
        Validators.minLength(3)
      ]),
      email: this.fb.nonNullable.control('', [
        Validators.required,
        Validators.email,
        this.ufcEmailValidator()
      ]),
      password: this.fb.nonNullable.control('', [
        Validators.required,
        Validators.minLength(6)
      ]),
      confirmPassword: this.fb.nonNullable.control('', [Validators.required]),
      phone: this.fb.control<string | null>(null, [this.phoneOptionalValidator()])
    }, { validators: this.passwordsMatchValidator });
  }

  get f() {
    return this.registerForm.controls;
  }

  register(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    const { fullName, userName, email, password, phone } = this.registerForm.getRawValue();

    this.isLoading = true;
    this.errorMessage = null;

    const phoneDigits = this.digitsOnly(phone ?? '');

    // envia ambos: fullName e userName, conforme você adicionou no backend
    const payload: RegisterRequest = {
      fullName: fullName.trim(),
      userName: userName.trim(),
      mail: email.trim(),
      password,
      phone: phoneDigits || undefined
    };

    this.authService.register(payload).subscribe({
      next: (response: AccountCreatedResponse) => {
        const message = `Conta criada com sucesso. ID: ${response.id}, criada em: ${new Date(response.createdAt).toLocaleDateString('pt-BR')}`;
        this.router.navigate(['/auth/login'], { queryParams: { message } });
      },
      error: (err) => {
        this.errorMessage = err?.message || 'Erro ao registrar. Tente novamente.';
        console.log(err)
        this.isLoading = false;
      },
      complete: () => this.isLoading = false
    });
  }

  private fullNameValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const v = (control.value || '').toString().trim().replace(/\s+/g, ' ');
      if (!v) return null;
      const parts = v.split(' ').filter(Boolean);
      if (parts.length < 2) return { fullNameInvalid: true };
      if (parts[0].length < 2 || parts[1].length < 2) return { fullNameInvalid: true };
      return null;
    };
  }

  private phoneOptionalValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const digits = this.digitsOnly(control.value || '');
      if (digits.length === 0) return null; // opcional
      return digits.length === 11 ? null : { phoneInvalid: true };
    };
  }

  onPhoneKeydown(e: KeyboardEvent) {
    const allowedControl = [
      'Backspace', 'Delete', 'Tab', 'Enter', 'Escape', 'ArrowLeft', 'ArrowRight', 'Home', 'End'
    ];
    if (allowedControl.includes(e.key) || (e.ctrlKey || e.metaKey)) return;
    if (!/^[0-9]$/.test(e.key)) e.preventDefault();
  }

  onPhoneInput() {
    const raw = this.f.phone.value ?? '';
    const digits = this.digitsOnly(raw).slice(0, 11); // 2 DDD + 9 número
    const formatted = this.formatPhone(digits);
    if (formatted !== raw) {
      this.f.phone.setValue(formatted, { emitEvent: false });
    }
  }

  onPhonePaste(e: ClipboardEvent) {
    e.preventDefault();
    const pasted = e.clipboardData?.getData('text') ?? '';
    const digits = this.digitsOnly(pasted).slice(0, 11);
    this.f.phone.setValue(this.formatPhone(digits));
  }

  private formatPhone(d: string): string {
    const ddd = d.slice(0, 2);
    const num = d.slice(2); // até 9 dígitos
    let numFmt = num;
    if (num.length > 5) numFmt = `${num.slice(0, 5)}-${num.slice(5)}`;
    return ddd.length ? `${ddd} - ${numFmt}` : numFmt;
  }

  private digitsOnly(s: string): string {
    return s.replace(/\D+/g, '');
  }

  private ufcEmailValidator(): ValidatorFn {
    const re = /^[^@]+@(ufc\.br|alu\.ufc\.br)$/i;
    return (control: AbstractControl): ValidationErrors | null => {
      const v = (control.value || '').toString().trim();
      if (!v) return null;
      return re.test(v) ? null : { ufcEmail: true };
    };
  }

  private passwordsMatchValidator: ValidatorFn = (group: AbstractControl): ValidationErrors | null => {
    const p = group.get('password')?.value;
    const c = group.get('confirmPassword')?.value;
    return p && c && p !== c ? { passwordMismatch: true } : null;
  };
}
