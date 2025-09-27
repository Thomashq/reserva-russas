import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { RoomsService } from '../rooms.service';
import { Router } from '@angular/router';
import { MatListModule } from '@angular/material/list';

@Component({
  selector: 'app-room-list',
  imports: [CommonModule, MatButtonModule, MatCardModule, MatListModule],
  templateUrl: './room-list.component.html',
  styleUrl: './room-list.component.css'
})
export class RoomListComponent implements OnInit{
  constructor(
    private rommService: RoomsService,
    private router: Router
  ) { }

  isLoading = false;

  rooms: any[] = [];

  ngOnInit(): void {
    this.isLoading = true;
    this.rommService.GetAll().subscribe({
      next: (result) => {
        this.rooms = result;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('There was an error!', error);
        this.isLoading = false;
      }
    });
  }

  openRoom(rooms: any) {
    this.router.navigate(['/rooms', rooms.Id]);
  }

  newReservation(rooms: any) {
    this.router.navigate(['/reservations/new', { roomId: rooms.Id }]);
  }
}
