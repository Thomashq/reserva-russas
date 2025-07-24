import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../auth.service';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { LoginRequest } from '../../domain/dto/request/LoginRequest';

@Component({
  selector: 'app-login',
  templateUrl: './auth-login.component.html',
  standalone: false,
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
    this.loginForm = this.fb.group({
      userName: ['', [Validators.required]],
      password: ['', [Validators.required]],
    });
  }

  onSubmit(): void {
    if (this.loginForm.invalid) return;

    this.isLoading = true;
    const credentials: LoginRequest = this.loginForm.value;

    this.authService.login(credentials).subscribe({
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
