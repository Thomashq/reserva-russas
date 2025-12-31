import { FooterComponent } from './footer/footer.component';
import { HeaderComponent } from './header/header.component';
import { RouterModule, Router, NavigationEnd } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Component, HostBinding } from '@angular/core';
import { ThemeService } from './theme/theme-service';
import { SideMenuComponent } from './side-menu/side-menu.component';
import { AuthService } from './auth/auth.service';
import { Account } from './domain/models/account';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-root',
  standalone: true,
  templateUrl: './app.component.html',
  imports: [CommonModule, RouterModule, HeaderComponent, FooterComponent, SideMenuComponent],
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  showLayout = true;
  account: Account | null = null;
  accountId = 0;
  private destroy$ = new Subject<void>();

  constructor(
    private router: Router,
    private themeService: ThemeService,
    private authService: AuthService
  ) {
    this.router.events.subscribe((event: any) => {
      if (event instanceof NavigationEnd) {
        this.showLayout = !['/auth', '/auth/login', '/login', '/auth/register'].includes(event.url);
      }
    });
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

  // aplica a classe do tema no <app-root>:
  @HostBinding('class.custom-theme-light') get isLight() {
    return this.themeService.theme === 'light';
  }

  @HostBinding('class.custom-theme-dark') get isDark() {
    return this.themeService.theme === 'dark';
  }
}
