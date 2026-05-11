import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { map } from 'rxjs/operators';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { RoomsApiService } from '../../../core/services/rooms-api.service';
import type { RoomDto } from '../../../core/models/room.models';
import { AppCardComponent } from '../../../shared/ui/app-card/app-card.component';
import { AppLoaderComponent } from '../../../shared/ui/app-loader/app-loader.component';
import { AppBadgeComponent } from '../../../shared/ui/app-badge/app-badge.component';

@Component({
  selector: 'app-room-detail',
  standalone: true,
  imports: [RouterLink, MatButtonModule, MatChipsModule, AppCardComponent, AppLoaderComponent, AppBadgeComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="mx-auto max-w-4xl space-y-6 px-4 py-12 text-neutral-900 dark:text-neutral-50">
      @if (loading()) {
        <app-loader />
      } @else if (room()) {
        @let r = room()!;
        <div class="flex flex-wrap items-start justify-between gap-4">
          <div>
            <p class="text-sm text-neutral-500 dark:text-neutral-400">{{ r.hotelName }}</p>
            <h1 class="text-3xl font-semibold tracking-tight">Room {{ r.roomNumber }}</h1>
            <div class="mt-2 flex flex-wrap gap-2">
              <app-badge tone="info">{{ r.type }}</app-badge>
              <app-badge>{{ r.status }}</app-badge>
            </div>
          </div>
          <a routerLink="/rooms/search">
            <button mat-stroked-button color="primary" type="button">Back to search</button>
          </a>
        </div>
        <app-card title="Overview">
          <p class="text-sm leading-relaxed text-neutral-600 dark:text-neutral-300">
            {{ r.description || 'Premium room with curated amenities.' }}
          </p>
          <dl class="mt-4 grid gap-3 text-sm sm:grid-cols-2">
            <div>
              <dt class="text-neutral-500 dark:text-neutral-400">Capacity</dt>
              <dd class="font-medium">{{ r.capacity }} guests</dd>
            </div>
            <div>
              <dt class="text-neutral-500 dark:text-neutral-400">Floor</dt>
              <dd class="font-medium">{{ r.floorNumber }}</dd>
            </div>
            <div>
              <dt class="text-neutral-500 dark:text-neutral-400">Off-peak from</dt>
              <dd class="font-medium">£{{ r.priceOffPeak }}</dd>
            </div>
            <div>
              <dt class="text-neutral-500 dark:text-neutral-400">Peak from</dt>
              <dd class="font-medium">£{{ r.pricePeak }}</dd>
            </div>
          </dl>
        </app-card>
        <div class="flex flex-wrap gap-3">
          <a routerLink="/login">
            <button mat-flat-button color="primary" type="button">Sign in to book</button>
          </a>
        </div>
      }
    </div>
  `,
})
export class RoomDetailComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly roomsApi = inject(RoomsApiService);

  readonly room = signal<RoomDto | null>(null);
  readonly loading = signal(true);

  constructor() {
    this.route.paramMap.pipe(map((p) => Number(p.get('id')))).subscribe((id) => {
      if (!Number.isFinite(id)) {
        this.loading.set(false);
        return;
      }
      this.loading.set(true);
      this.roomsApi.getById(id).subscribe({
        next: (r) => {
          this.room.set(r);
          this.loading.set(false);
        },
        error: () => {
          this.room.set(null);
          this.loading.set(false);
        },
      });
    });
  }
}
