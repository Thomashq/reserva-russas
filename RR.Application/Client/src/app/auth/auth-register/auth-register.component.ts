import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../auth.service';
import { Router } from '@angular/router';
import { RegisterRequest } from '../../domain/dto/request/RegisterRequest';
import { AccountCreatedResponse } from '../../domain/dto/response/AccountResponse';


@Component({
  selector: 'app-register',
  templateUrl: './auth-register.component.html',
  standalone: false,
  styleUrls: ['./auth-register.component.css']
})
export class RegisterComponent {
  registerForm: FormGroup;
  errorMessage: string | null = null;
  isLoading: boolean = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.registerForm = this.fb.group({
      userName: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required],
      phone: [''] // Campo opcional para telefone
    });
  }

  register(): void {
    if (this.registerForm.invalid) {
      this.markFormGroupTouched();
      return;
    }

    const { userName, email, password, confirmPassword, phone } = this.registerForm.value;

    if (password !== confirmPassword) {
      this.errorMessage = 'As senhas não coincidem.';
      return;
    }

    this.isLoading = true;
    this.errorMessage = null;

    const registerData: RegisterRequest = {
      userName: userName,
      mail: email, // Mapeando 'email' para 'mail' para coincidir com a controller
      password: password,
      phone: phone || undefined, // Enviar apenas se preenchido
      accountPermission: 0 // Usuário comum
    };

    this.authService.register(registerData).subscribe({
      next: (response: AccountCreatedResponse) => {
        console.log('Registro bem-sucedido:', response);
        // Agora temos acesso ao ID da conta criada e data de criação
        const message = `Conta criada com sucesso! ID: ${response.id}, criada em: ${new Date(response.createdAt).toLocaleDateString('pt-BR')}`;
        this.router.navigate(['/auth/login'], {
          queryParams: { message }
        });
      },
      error: (err) => {
        console.error('Erro no registro:', err);
        this.errorMessage = err.message || 'Erro ao registrar. Tente novamente.';
        this.isLoading = false;
      },
      complete: () => {
        this.isLoading = false;
      }
    });
  }

  private markFormGroupTouched(): void {
    Object.keys(this.registerForm.controls).forEach(key => {
      this.registerForm.get(key)?.markAsTouched();
    });
  }

  // Métodos auxiliares para validação no template
  hasError(fieldName: string, errorType: string): boolean {
    const field = this.registerForm.get(fieldName);
    return !!(field && field.hasError(errorType) && field.touched);
  }

  getErrorMessage(fieldName: string): string {
    const field = this.registerForm.get(fieldName);
    if (!field || !field.errors || !field.touched) return '';

    if (field.hasError('required')) return `${fieldName} é obrigatório`;
    if (field.hasError('email')) return 'Email inválido';
    if (field.hasError('minlength')) {
      const requiredLength = field.errors['minlength'].requiredLength;
      return `Deve ter pelo menos ${requiredLength} caracteres`;
    }

    return 'Campo inválido';
  }
}
