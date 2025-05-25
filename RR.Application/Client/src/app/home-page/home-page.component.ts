import { Component } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home-page.component.html',
  standalone: false,
  styleUrl: './home-page.component.css'
})
export class HomePageComponent {
  heroTitle = 'Lorem ipsum dolor sit amet, consectetur adipiscing elit.';
  heroText = 'Vivamus ut vehicula arcu, id consequat nisi. Curabitur tristique mi consequat et ultricies integer tempus.';
}
