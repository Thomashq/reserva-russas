import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { ReservationListComponent } from './reservation/reservation-list/reservation-list.component';
import { ReservationNewDialogComponent } from './reservation/reservation-new-dialog/reservation-new-dialog.component';

@Component({
  selector: 'app-home-page',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatToolbarModule,
    MatCardModule,
    MatButtonModule,
    ReservationListComponent
  ],
  templateUrl: './home-page.component.html',
  styleUrls: ['./home-page.component.css']
})
export class HomePageComponent {
  constructor(private dialog: MatDialog) { }

  openNewReservationDialog(): void {
    const dialogRef = this.dialog.open(ReservationNewDialogComponent, {
      width: '800px',
      maxHeight: '90vh',
      disableClose: true
    });

    // The reservation list will auto-refresh when dialog closes with success
  }
}
