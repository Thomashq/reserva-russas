import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../auth/auth.service';
import { Observable } from 'rxjs';
import { Account } from '../domain/models/account';

@Component({
  selector: 'app-header',
  standalone: true,
  templateUrl: './header.component.html',
  imports: [MatButtonModule, MatToolbarModule, CommonModule, RouterModule],
  styleUrls: ['./header.component.css']
})
export class HeaderComponent {
  isAuth$!: Observable<boolean>;
  user$!: Observable<Account | null>;

  constructor(private router: Router, private authService: AuthService) {
    this.isAuth$ = this.authService.isAuthenticated$;
    this.user$ = this.authService.currentUser$;     
  }

  navigateToInfo(): void { console.log('Navegando para Info'); }
  navigateToAbout(): void { console.log('Navegando para Sobre'); }
  navigateToRooms(): void { console.log('Navegando para Salas'); }
  navigateToContact(): void { console.log('Navegando para Contato'); }

  register(): void { this.router.navigate(['/auth/register']); }
  login(): void { this.router.navigate(['/auth']); }
  logout(): void { this.authService.logout(); }
}
