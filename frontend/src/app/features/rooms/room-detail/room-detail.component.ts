import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { map } from 'rxjs/operators';
import { MatChipsModule } from '@angular/material/chips';
import { RoomsApiService } from '../../../core/services/rooms-api.service';
import type { RoomDto } from '../../../core/models/room.models';
import { AppCardComponent } from '../../../shared/ui/app-card/app-card.component';
import { AppLoaderComponent } from '../../../shared/ui/app-loader/app-loader.component';
import { AppBadgeComponent } from '../../../shared/ui/app-badge/app-badge.component';
import { AppButtonComponent } from '../../../shared/ui/app-button/app-button.component';

@Component({
  selector: 'app-room-detail',
  standalone: true,
  imports: [RouterLink, MatChipsModule, AppCardComponent, AppLoaderComponent, AppBadgeComponent, AppButtonComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="mx-auto max-w-4xl space-y-6 px-4 py-12 text-zinc-900">
      @if (loading()) {
        <app-loader />
      } @else if (room()) {
        @let r = room()!;
        <div class="flex flex-wrap items-start justify-between gap-4">
          <div>
            <p class="text-sm text-zinc-500">{{ r.hotelName }}</p>
            <h1 class="text-3xl font-semibold tracking-tight">Room {{ r.roomNumber }}</h1>
            <div class="mt-2 flex flex-wrap gap-2">
              <app-badge tone="info">{{ r.type }}</app-badge>
              <app-badge>{{ r.status }}</app-badge>
            </div>
          </div>
          <a routerLink="/rooms/search" class="inline-block">
            <app-button variant="secondary" type="button">Back to search</app-button>
          </a>
        </div>
        <app-card title="Overview">
          <p class="text-sm leading-relaxed text-zinc-600">
            {{ r.description || 'Premium room with curated amenities.' }}
          </p>
          <dl class="mt-4 grid gap-3 text-sm sm:grid-cols-2">
            <div>
              <dt class="text-zinc-500">Capacity</dt>
              <dd class="font-medium">{{ r.capacity }} guests</dd>
            </div>
            <div>
              <dt class="text-zinc-500">Floor</dt>
              <dd class="font-medium">{{ r.floorNumber }}</dd>
            </div>
            <div>
              <dt class="text-zinc-500">Off-peak from</dt>
              <dd class="font-medium">£{{ r.priceOffPeak }}</dd>
            </div>
            <div>
              <dt class="text-zinc-500">Peak from</dt>
              <dd class="font-medium">£{{ r.pricePeak }}</dd>
            </div>
          </dl>
        </app-card>
        <div class="flex flex-wrap gap-3">
          <a routerLink="/login" class="inline-block">
            <app-button variant="primary" type="button">Sign in to book</app-button>
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
