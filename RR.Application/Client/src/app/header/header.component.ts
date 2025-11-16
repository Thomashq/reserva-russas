import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../auth/auth.service';
import { Subject, takeUntil } from 'rxjs';
import { Account } from '../domain/models/account';
import { HeaderService } from './header.service';
import { ThemeService } from '../theme/theme-service';
import { MatIcon, MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-header',
  standalone: true,
  templateUrl: './header.component.html',
  imports: [MatButtonModule, MatToolbarModule, CommonModule, RouterModule, MatIconModule],
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit, OnDestroy {
  account: Account | null = null;
  isAuth: boolean = false;
  private destroy$ = new Subject<void>();

  get userName(): string {
    return this.account?.UserName || this.account?.Mail || 'Usuário';
  }

  constructor(
    private router: Router,
    private headerService: HeaderService,
    private authService: AuthService,
    private themeService: ThemeService
  ) { }

  ngOnInit(): void {
    this.authService.currentUser$
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (account) => {
          this.account = account;
          this.isAuth = !!account && Object.keys(account).length > 0;
        },
        error: (error) => {
          this.account = null;
          this.isAuth = false;
        }
      });
  }


  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  toggleTheme() {
    this.themeService.toggle();
  }
  get isDark() {
    return this.themeService.theme === 'dark';
  }

  // Navegação
  navigateToInfo(): void {
    this.router.navigate(['/']);
  }

  navigateToAbout(): void {
    console.log('Navegando para Sobre');
  }

  navigateToRooms(): void {
    this.router.navigate(['/room/list']);
  }

  navigateToContact(): void {
    console.log('Navegando para Contato');
  }

  register(): void {
    this.router.navigate(['/auth/register']);
  }

  login(): void {
    this.router.navigate(['/auth/login']);
  }

  logout(): void {
    this.authService.logout().subscribe({
      next: () => {
        this.router.navigate(['/auth/login']);
      },
      error: (error) => {
        this.router.navigate(['/auth/login']);
      }
    });
  }
}
