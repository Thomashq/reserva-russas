import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { MatListModule } from '@angular/material/list';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { AuthService } from '../auth/auth.service';
import { Account } from '../domain/models/account';
import { FormsModule } from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-side-menu',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    MatListModule,
    MatExpansionModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
  ],
  templateUrl: './side-menu.component.html',
  styleUrls: ['./side-menu.component.css']
})
export class SideMenuComponent implements OnInit, OnDestroy {
  account: Account | null = null;
  accountId = 0;

  searchText = '';

  private destroy$ = new Subject<void>();

  constructor(
    private authService: AuthService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.authService.currentUser$
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (result) => {
          this.account = result;
          this.accountId = result?.id ?? 0;
        },
        error: () => {
          this.account = null;
          this.accountId = 0;
        }
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  navigateToRooms(): void {
    this.router.navigate(['/room/list']);
  }

  logout(): void {
    this.authService.logout().subscribe({
      next: () => {
        // nada aqui: o AuthService já limpou o estado e o menu
        // troca automaticamente para o modo "sem usuário"
      },
      error: (error) => {
        console.error('Erro ao fazer logout no side-menu', error);
      }
    });
  }

  onSearchEnter(): void {
    if (!this.searchText?.trim()) return;

    this.router.navigate(['/busca'], {
      queryParams: { q: this.searchText.trim() }
    });
  }

  getPermissionLabel(acc: Account | null): string {
    if (!acc) return 'Conta';

    const perm = (acc as any).permission ?? (acc as any).AccountPermission;

    switch (perm) {
      case 0: // EAccountPermission.Manager
        return 'Gerente de Sala';
      case 1: // EAccountPermission.Servant
        return 'Servidor';
      case 2: // EAccountPermission.Student
        return 'Aluno';
      case 3:
        return 'Admin';
      default:
        return 'Conta';
    }
  }
}

