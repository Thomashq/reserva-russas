import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService, LoginRequest } from '../auth.service';

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  loading = false;
  error = '';
  hidePassword = true;

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.loginForm = this.formBuilder.group({
      userName: ['', [Validators.required]],
      senha: ['', [Validators.required, Validators.minLength(3)]]
    });
  }

  ngOnInit(): void {
    // Verifica se o usuário já está autenticado
    console.log("Componente nao carregado");
    if (this.authService.isAuthenticated()) {
      console.log('Usuário já autenticado, redirecionando...');
      this.router.navigate(['/']); // Redireciona para home
    }
  }

  onSubmit(): void {
    if (this.loginForm.valid) {
      this.loading = true;
      this.error = '';

      const credentials: LoginRequest = {
        userName: this.loginForm.get('userName')?.value,
        senha: this.loginForm.get('senha')?.value
      };

      this.authService.login(credentials).subscribe({
        next: (token) => {
          this.loading = false;
          console.log('Login realizado com sucesso');

          // Obtém o usuário atual para mostrar informações
          const currentUser = this.authService.getCurrentUser();
          console.log('Usuário logado:', currentUser);

          // Redireciona após login bem-sucedido
          this.router.navigate(['/']); // Redireciona para home
        },
        error: (error) => {
          this.loading = false;

          // Trata diferentes tipos de erro
          if (error.status === 401) {
            this.error = 'Usuário ou senha incorretos';
          } else if (error.error?.message) {
            this.error = error.error.message;
          } else if (error.message) {
            this.error = error.message;
          } else {
            this.error = 'Erro ao fazer login. Tente novamente.';
          }

          console.error('Erro no login:', error);
        }
      });
    } else {
      this.markFormGroupTouched();
    }
  }

  onGoogleLogin(): void {
    this.authService.loginWithGoogle();
  }

  navigateToRegister(): void {
    this.router.navigate(['/auth/register']);
  }

  private markFormGroupTouched(): void {
    Object.keys(this.loginForm.controls).forEach(key => {
      const control = this.loginForm.get(key);
      control?.markAsTouched();
    });
  }

  // Getters para facilitar o acesso aos campos no template
  get userName() {
    return this.loginForm.get('userName');
  }

  get senha() {
    return this.loginForm.get('senha');
  }
}
