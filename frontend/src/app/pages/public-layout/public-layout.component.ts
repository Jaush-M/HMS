import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-public-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="min-h-screen bg-white">
      <header
        class="sticky top-0 z-20 border-b border-zinc-100 bg-white/95 backdrop-blur-md"
        role="banner"
      >
        <div class="mx-auto flex max-w-6xl items-center justify-between gap-4 px-6 py-4">
          <a routerLink="/" class="flex items-center gap-2 font-bold tracking-tight text-zinc-900 text-[15px]">
            <span class="material-icons-outlined text-cyan-500" aria-hidden="true">apartment</span>
            Grand Plaza
          </a>
          <nav
            class="hidden items-center gap-0.5 rounded-full border border-zinc-100 bg-zinc-50 px-2 py-1 text-[13px] md:flex"
            aria-label="Site navigation"
          >
            <a
              routerLink="/"
              routerLinkActive="bg-zinc-900 text-white"
              [routerLinkActiveOptions]="{ exact: true }"
              class="rounded-full px-3.5 py-1.5 font-medium text-zinc-600 hover:bg-zinc-100 transition-colors"
            >Home</a>
            <a
              routerLink="/rooms/search"
              routerLinkActive="bg-zinc-900 text-white"
              class="rounded-full px-3.5 py-1.5 font-medium text-zinc-600 hover:bg-zinc-100 transition-colors"
            >Rooms</a>
            <a
              routerLink="/hotel"
              routerLinkActive="bg-zinc-900 text-white"
              class="rounded-full px-3.5 py-1.5 font-medium text-zinc-600 hover:bg-zinc-100 transition-colors"
            >Hotel</a>
            <a
              routerLink="/contact"
              routerLinkActive="bg-zinc-900 text-white"
              class="rounded-full px-3.5 py-1.5 font-medium text-zinc-600 hover:bg-zinc-100 transition-colors"
            >Contact</a>
          </nav>
          <div class="flex items-center gap-2">
            <a
              routerLink="/login"
              class="rounded-full border border-zinc-200 px-4 py-2 text-[13px] font-medium text-zinc-700 hover:bg-zinc-50 transition-colors"
            >Sign in</a>
            <a
              routerLink="/register"
              class="rounded-full bg-zinc-900 px-4 py-2 text-[13px] font-semibold text-white hover:bg-zinc-800 transition-colors"
            >Register</a>
          </div>
        </div>
      </header>
      <router-outlet />
    </div>
  `,
})
export class PublicLayoutComponent {}
