import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { AuthService } from '../auth/auth.service';

@Component({
  selector: 'app-home',
  templateUrl: './home-page.component.html',
  imports: [CommonModule, RouterModule],
  styleUrl: './home-page.component.css'
})
export class HomePageComponent {
  constructor(private authService : AuthService) { }
  heroTitle = 'Lorem ipsum dolor sit amet, consectetur adipiscing elit.';
  heroText = 'Vivamus ut vehicula arcu, id consequat nisi. Curabitur tristique mi consequat et ultricies integer tempus.';
}
