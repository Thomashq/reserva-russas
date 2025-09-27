import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../auth/auth.service';

@Component({
  selector: 'app-header',
  standalone: true,
  templateUrl: './header.component.html',
  imports: [MatButtonModule, MatToolbarModule, CommonModule, RouterModule],
  styleUrls: ['./header.component.css']
})
export class HeaderComponent {

  constructor(private router: Router, private authService: AuthService) { }

  // Alinhado ao AuthService atual
  get isAuth(): boolean {
    return this.authService.isLoggedIn();
  }
  get userName(): string {
    return this.authService.Account?.userName ?? '';
  }

  // Navegação
  navigateToInfo(): void { this.router.navigate(['/']); }
  navigateToAbout(): void { /* placeholder */ console.log('Navegando para Sobre'); }
  navigateToRooms(): void { this.router.navigate(['/room/list']); }
  navigateToContact(): void { /* placeholder */ console.log('Navegando para Contato'); }

  register(): void { this.router.navigate(['/auth/register']); }
  login(): void { this.router.navigate(['/auth/login']); }
  logout(): void { this.authService.logout(); }
}
