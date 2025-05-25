// footer.component.ts
import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html',
  standalone: false,
  styleUrls: ['./footer.component.css']
})
export class FooterComponent {
  currentYear = new Date().getFullYear();

  constructor(private router: Router) { }

  navigateToInfo(): void {
    this.router.navigate(['/']);
  }

  navigateToAbout(): void {
    this.router.navigate(['/sobre']);
  }

  navigateToRooms(): void {
    this.router.navigate(['/salas']);
  }

  navigateToContact(): void {
    this.router.navigate(['/contato']);
  }

  callPhone(phone: string): void {
    window.open(`tel:${phone}`, '_self');
  }

  openWhatsApp(phone: string): void {
    const message = encodeURIComponent('Olá! Gostaria de mais informações sobre as salas.');
    window.open(`https://wa.me/55${phone}?text=${message}`, '_blank');
  }

  openInstagram(): void {
    window.open('https://instagram.com/ufc', '_blank');
  }

  openTwitter(): void {
    window.open('https://twitter.com/ufc', '_blank');
  }

  openMaps(): void {
    const address = encodeURIComponent('Endereço Aqui, 465,78');
    window.open(`https://www.google.com/maps/search/?api=1&query=${address}`, '_blank');
  }
}
