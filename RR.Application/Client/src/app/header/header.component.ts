import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  standalone:false,
  styleUrls: ['./header.component.css']
})
export class HeaderComponent {

  constructor(private router: Router) { }

  // Métodos para navegação (placeholder)
  navigateToInfo(): void {
    // this.router.navigate(['/info']);
    console.log('Navegando para Info');
  }

  navigateToAbout(): void {
    // this.router.navigate(['/sobre']);
    console.log('Navegando para Sobre');
  }

  navigateToRooms(): void {
    // this.router.navigate(['/salas']);
    console.log('Navegando para Salas');
  }

  navigateToContact(): void {
    // this.router.navigate(['/contato']);
    console.log('Navegando para Contato');
  }

  register(): void {
    // this.router.navigate(['/registro']);
    this.router.navigate(['/auth/register']);
  }

  login(): void {
    this.router.navigate(['/auth']);
  }
}
