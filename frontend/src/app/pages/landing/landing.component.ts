import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-landing',
  standalone: true,
  imports: [RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <section
      class="relative flex min-h-[calc(100vh-5rem)] flex-col justify-end overflow-hidden px-4 pb-16 pt-10 md:px-8 md:pb-24"
    >
      <img
        src="https://images.unsplash.com/photo-1506905925346-21bda4d32df4?auto=format&fit=crop&w=2000&q=80"
        alt=""
        class="pointer-events-none absolute inset-0 h-full w-full object-cover opacity-40"
        loading="eager"
      />
      <div
        class="pointer-events-none absolute inset-0 bg-gradient-to-t from-black via-black/60 to-transparent"
        aria-hidden="true"
      ></div>
      <div class="relative z-10 mx-auto w-full max-w-4xl">
        <p class="text-xs font-semibold uppercase tracking-[0.35em] text-blue-200/90">
          Grand Plaza Collection
        </p>
        <h1 class="mt-4 text-4xl font-semibold tracking-tight text-white md:text-6xl">
          Refined stays, orchestrated in one place.
        </h1>
        <p class="mt-4 max-w-xl text-sm leading-relaxed text-white/80 md:text-base">
          Discover availability, manage bookings, and run front-of-house operations with a calm,
          enterprise-grade experience inspired by modern travel platforms.
        </p>
        <div class="mt-8 flex flex-wrap gap-3">
          <a
            routerLink="/rooms/search"
            class="inline-flex items-center justify-center rounded-lg bg-blue-600 px-5 py-2.5 text-sm font-semibold text-white hover:bg-blue-700"
            >Search rooms</a
          >
          <a
            routerLink="/hotel"
            class="inline-flex items-center justify-center rounded-lg border border-white/30 bg-white/10 px-5 py-2.5 text-sm font-semibold text-white hover:bg-white/20"
            >Hotel information</a
          >
        </div>
      </div>
    </section>
  `,
})
export class LandingComponent {}
