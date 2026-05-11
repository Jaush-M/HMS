import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { HotelsApiService } from '../../../core/services/hotels-api.service';
import { RoomsApiService } from '../../../core/services/rooms-api.service';
import { environment } from '../../../../environments/environment';
import type { HotelSummaryDto } from '../../../core/models/hotel.models';
import type { RoomDto } from '../../../core/models/room.models';
import { AppCardComponent } from '../../../shared/ui/app-card/app-card.component';
import { AppLoaderComponent } from '../../../shared/ui/app-loader/app-loader.component';
import { AppEmptyStateComponent } from '../../../shared/ui/app-empty-state/app-empty-state.component';
import { AppTableComponent } from '../../../shared/ui/app-table/app-table.component';

@Component({
  selector: 'app-room-search',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatSelectModule,
    MatTableModule,
    MatButtonModule,
    AppCardComponent,
    AppLoaderComponent,
    AppEmptyStateComponent,
    AppTableComponent,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="mx-auto max-w-6xl space-y-6 px-4 py-12 text-neutral-900 dark:text-neutral-50">
      <h1 class="text-3xl font-semibold tracking-tight">Room availability</h1>
      <app-card title="Search">
        <form class="mt-4 grid gap-4 md:grid-cols-2 lg:grid-cols-4" [formGroup]="form" (ngSubmit)="search()">
          <mat-form-field appearance="outline">
            <mat-label>Hotel</mat-label>
            <mat-select formControlName="hotelId">
              @for (h of hotels(); track h.id) {
                <mat-option [value]="h.id">{{ h.name }}</mat-option>
              }
            </mat-select>
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>Check-in</mat-label>
            <input matInput [matDatepicker]="ci" formControlName="checkIn" />
            <mat-datepicker-toggle matIconSuffix [for]="ci" />
            <mat-datepicker #ci />
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>Check-out</mat-label>
            <input matInput [matDatepicker]="co" formControlName="checkOut" />
            <mat-datepicker-toggle matIconSuffix [for]="co" />
            <mat-datepicker #co />
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>Min guests</mat-label>
            <input matInput type="number" formControlName="minCapacity" min="1" />
          </mat-form-field>
          <div class="flex items-end md:col-span-2 lg:col-span-4">
            <button mat-flat-button color="primary" type="submit" [disabled]="loading()">
              Search availability
            </button>
          </div>
        </form>
      </app-card>
      @if (loading()) {
        <app-loader caption="Searching rooms…" />
      } @else if (rooms().length === 0 && searched()) {
        <app-empty-state
          icon="hotel"
          title="No rooms found"
          hint="Try different dates or a lower minimum capacity."
        />
      } @else if (rooms().length) {
        <app-table>
          <table mat-table [dataSource]="rooms()" class="w-full">
            <ng-container matColumnDef="roomNumber">
              <th mat-header-cell *matHeaderCellDef>Room</th>
              <td mat-cell *matCellDef="let r">{{ r.roomNumber }}</td>
            </ng-container>
            <ng-container matColumnDef="type">
              <th mat-header-cell *matHeaderCellDef>Type</th>
              <td mat-cell *matCellDef="let r">{{ r.type }}</td>
            </ng-container>
            <ng-container matColumnDef="capacity">
              <th mat-header-cell *matHeaderCellDef>Capacity</th>
              <td mat-cell *matCellDef="let r">{{ r.capacity }}</td>
            </ng-container>
            <ng-container matColumnDef="price">
              <th mat-header-cell *matHeaderCellDef>From</th>
              <td mat-cell *matCellDef="let r">£{{ r.priceOffPeak }}</td>
            </ng-container>
            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef></th>
              <td mat-cell *matCellDef="let r">
                <a mat-button color="primary" [routerLink]="['/rooms', r.id]">Details</a>
              </td>
            </ng-container>
            <tr mat-header-row *matHeaderRowDef="cols"></tr>
            <tr mat-row *matRowDef="let row; columns: cols"></tr>
          </table>
        </app-table>
      }
    </div>
  `,
})
export class RoomSearchComponent {
  private readonly fb = inject(FormBuilder);
  private readonly hotelsApi = inject(HotelsApiService);
  private readonly roomsApi = inject(RoomsApiService);

  readonly cols = ['roomNumber', 'type', 'capacity', 'price', 'actions'];
  readonly hotels = signal<HotelSummaryDto[]>([]);
  readonly rooms = signal<RoomDto[]>([]);
  readonly loading = signal(false);
  readonly searched = signal(false);

  readonly form = this.fb.nonNullable.group({
    hotelId: [environment.defaultHotelId, Validators.required],
    checkIn: [new Date(), Validators.required],
    checkOut: [new Date(Date.now() + 86400000), Validators.required],
    minCapacity: [1, [Validators.required, Validators.min(1)]],
  });

  constructor() {
    this.hotelsApi.getAll().subscribe({
      next: (h) => {
        this.hotels.set(h);
        if (h.length && !this.form.controls.hotelId.value) {
          this.form.patchValue({ hotelId: h[0]!.id });
        }
      },
      error: () => this.hotels.set([]),
    });
  }

  search(): void {
    if (this.form.invalid) return;
    const v = this.form.getRawValue();
    const checkIn = this.toYmd(v.checkIn);
    const checkOut = this.toYmd(v.checkOut);
    if (checkOut <= checkIn) return;
    this.loading.set(true);
    this.searched.set(true);
    this.roomsApi
      .searchAvailable({
        hotelId: v.hotelId,
        checkIn,
        checkOut,
        minCapacity: v.minCapacity,
      })
      .subscribe({
        next: (r) => {
          this.rooms.set(r);
          this.loading.set(false);
        },
        error: () => {
          this.rooms.set([]);
          this.loading.set(false);
        },
      });
  }

  private toYmd(d: Date): string {
    return d.toISOString().slice(0, 10);
  }
}
