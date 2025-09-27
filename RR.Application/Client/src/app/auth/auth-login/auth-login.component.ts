import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';
import { ReactiveFormsModule } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../auth.service';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { LoginRequest } from '../../domain/dto/request/LoginRequest';

@Component({
  selector: 'app-login',
  templateUrl: './auth-login.component.html',
  imports: [MatDividerModule, MatIconModule, MatButtonModule, MatInputModule, MatFormFieldModule, MatCardModule, CommonModule, RouterModule, FormsModule, ReactiveFormsModule],
  styleUrls: ['./auth-login.component.css'],
})
export class LoginComponent implements OnInit {
  loginForm!: FormGroup;
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    localStorage.removeItem('auth_token');

    this.loginForm = this.fb.group({
      userName: ['', [Validators.required]],
      password: ['', [Validators.required]],
    });
  }

  onSubmit(): void {
    if (this.loginForm.invalid) return;
    this.isLoading = true;
    const credentials: LoginRequest = this.loginForm.value;
    this.authService.Login(credentials).subscribe({
      next: () => {
        this.snackBar.open('Login realizado com sucesso!', 'Fechar', { duration: 3000 });
        this.router.navigate(['/']); // ou rota de dashboard
      },
      error: (err) => {
        this.snackBar.open(err.message || 'Erro ao fazer login.', 'Fechar', { duration: 3000 });
        this.isLoading = false;
      },
      complete: () => this.isLoading = false
    });
  }
}
