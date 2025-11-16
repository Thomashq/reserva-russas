import { FooterComponent } from './footer/footer.component';
import { HeaderComponent } from './header/header.component';
import { RouterModule, Router, NavigationEnd } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Component, HostBinding } from '@angular/core';
import { ThemeService } from './theme/theme-service';
import { SideMenuComponent } from './side-menu/side-menu.component';

@Component({
  selector: 'app-root',
  standalone: true,
  templateUrl: './app.component.html',
  imports: [CommonModule, RouterModule, HeaderComponent, FooterComponent, SideMenuComponent],
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  showLayout = true;

  constructor(
    private router: Router,
    private themeService: ThemeService
  ) {
    this.router.events.subscribe((event: any) => {
      if (event instanceof NavigationEnd) {
        this.showLayout = !['/auth', '/auth/login', '/login', '/auth/register'].includes(event.url);
      }
    });
  }

  // aplica a classe do tema no <app-root>
  @HostBinding('class.custom-theme-light') get isLight() {
    return this.themeService.theme === 'light';
  }

  @HostBinding('class.custom-theme-dark') get isDark() {
    return this.themeService.theme === 'dark';
  }
}
