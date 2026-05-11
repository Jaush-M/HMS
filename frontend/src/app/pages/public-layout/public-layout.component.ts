import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-public-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="relative min-h-screen bg-neutral-950 text-white">
      <div
        class="pointer-events-none absolute inset-0 bg-[radial-gradient(ellipse_at_top,_rgba(59,130,246,0.15),_transparent_55%)]"
        aria-hidden="true"
      ></div>
      <header
        class="sticky top-0 z-20 border-b border-white/10 bg-black/30 backdrop-blur-md"
        role="banner"
      >
        <div class="mx-auto flex max-w-6xl items-center justify-between gap-4 px-4 py-4">
          <a routerLink="/" class="flex items-center gap-2 text-sm font-semibold tracking-tight">
            <span
              class="material-icons-outlined text-blue-300"
              aria-hidden="true"
              >apartment</span
            >
            Grand Plaza
          </a>
          <nav
            class="hidden items-center gap-1 rounded-full border border-white/10 bg-white/5 px-2 py-1 text-sm md:flex"
            aria-label="Marketing"
          >
            <a
              routerLink="/"
              routerLinkActive="bg-white text-neutral-900"
              [routerLinkActiveOptions]="{ exact: true }"
              class="rounded-full px-3 py-1.5 text-white/90 hover:bg-white/10"
              >Home</a
            >
            <a
              routerLink="/rooms/search"
              routerLinkActive="bg-white text-neutral-900"
              class="rounded-full px-3 py-1.5 text-white/90 hover:bg-white/10"
              >Rooms</a
            >
            <a
              routerLink="/hotel"
              routerLinkActive="bg-white text-neutral-900"
              class="rounded-full px-3 py-1.5 text-white/90 hover:bg-white/10"
              >Hotel</a
            >
            <a
              routerLink="/contact"
              routerLinkActive="bg-white text-neutral-900"
              class="rounded-full px-3 py-1.5 text-white/90 hover:bg-white/10"
              >Contact</a
            >
          </nav>
          <div class="flex items-center gap-2">
            <a
              routerLink="/login"
              class="rounded-full border border-white/20 px-4 py-2 text-sm font-medium text-white hover:bg-white/10"
              >Sign in</a
            >
            <a
              routerLink="/register"
              class="rounded-full bg-white px-4 py-2 text-sm font-semibold text-neutral-900 hover:bg-neutral-100"
              >Register</a
            >
          </div>
        </div>
      </header>
      <router-outlet />
    </div>
  `,
})
export class PublicLayoutComponent {}
