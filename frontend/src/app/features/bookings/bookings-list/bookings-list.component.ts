import { SlicePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { MatTableModule } from '@angular/material/table';
import { AuthService } from '../../../core/auth/auth.service';
import { BookingsApiService } from '../../../core/services/bookings-api.service';
import { HotelsApiService } from '../../../core/services/hotels-api.service';
import { environment } from '../../../../environments/environment';
import type { BookingDto } from '../../../core/models/booking.models';
import { AppCardComponent } from '../../../shared/ui/app-card/app-card.component';
import { AppTableComponent } from '../../../shared/ui/app-table/app-table.component';
import { AppLoaderComponent } from '../../../shared/ui/app-loader/app-loader.component';

@Component({
  selector: 'app-bookings-list',
  standalone: true,
  imports: [SlicePipe, MatTableModule, AppCardComponent, AppTableComponent, AppLoaderComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="space-y-4">
      <h1 class="text-2xl font-semibold text-neutral-900 dark:text-white">Bookings</h1>
      <app-card title="Directory">
        @if (loading()) {
          <app-loader />
        } @else {
          <app-table>
            <table mat-table [dataSource]="rows()" class="w-full">
              <ng-container matColumnDef="id">
                <th mat-header-cell *matHeaderCellDef>ID</th>
                <td mat-cell *matCellDef="let b">{{ b.id }}</td>
              </ng-container>
              <ng-container matColumnDef="guest">
                <th mat-header-cell *matHeaderCellDef>Guest</th>
                <td mat-cell *matCellDef="let b">{{ b.guestName }}</td>
              </ng-container>
              <ng-container matColumnDef="hotel">
                <th mat-header-cell *matHeaderCellDef>Hotel</th>
                <td mat-cell *matCellDef="let b">{{ b.hotelName }}</td>
              </ng-container>
              <ng-container matColumnDef="dates">
                <th mat-header-cell *matHeaderCellDef>Dates</th>
                <td mat-cell *matCellDef="let b">
                  {{ b.checkInDate | slice: 0 : 10 }} → {{ b.checkOutDate | slice: 0 : 10 }}
                </td>
              </ng-container>
              <ng-container matColumnDef="status">
                <th mat-header-cell *matHeaderCellDef>Status</th>
                <td mat-cell *matCellDef="let b">{{ b.status }}</td>
              </ng-container>
              <tr mat-header-row *matHeaderRowDef="cols"></tr>
              <tr mat-row *matRowDef="let row; columns: cols"></tr>
            </table>
          </app-table>
        }
      </app-card>
    </div>
  `,
})
export class BookingsListComponent {
  private readonly auth = inject(AuthService);
  private readonly bookingsApi = inject(BookingsApiService);
  private readonly hotelsApi = inject(HotelsApiService);

  readonly rows = signal<BookingDto[]>([]);
  readonly loading = signal(true);
  readonly cols = ['id', 'guest', 'hotel', 'dates', 'status'];

  constructor() {
    const role = this.auth.role();
    const uid = this.auth.userId();
    if (role === 'Guest' && uid != null) {
      this.bookingsApi.getByGuest(uid).subscribe((b) => {
        this.rows.set(b);
        this.loading.set(false);
      });
      return;
    }
    this.hotelsApi.getAll().subscribe({
      next: (h) => {
        const hid = h[0]?.id ?? environment.defaultHotelId;
        this.bookingsApi.getByHotel(hid).subscribe((b) => {
          this.rows.set(b);
          this.loading.set(false);
        });
      },
      error: () => {
        this.bookingsApi.getByHotel(environment.defaultHotelId).subscribe((b) => {
          this.rows.set(b);
          this.loading.set(false);
        });
      },
    });
  }
}
