import { FooterComponent } from './footer/footer.component';
import { HeaderComponent } from './header/header.component';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  imports: [CommonModule, RouterModule, HeaderComponent, FooterComponent],
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  showLayout = true;

  constructor(private router: Router) {
    this.router.events.subscribe((event: any )=> {
      if (event instanceof NavigationEnd) {
        this.showLayout = !['/auth', '/auth/login', '/login', '/auth/register'].includes(event.url);
      }
    });
  }
}
