import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AppButtonComponent } from '../../shared/ui/app-button/app-button.component';

@Component({
  selector: 'app-unauthorized',
  standalone: true,
  imports: [RouterLink, AppButtonComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <main
      class="flex min-h-screen flex-col items-center justify-center gap-4 bg-neutral-50 px-4 text-center dark:bg-neutral-950"
    >
      <p class="text-sm font-semibold uppercase tracking-widest text-amber-600 dark:text-amber-400">
        403
      </p>
      <h1 class="text-2xl font-semibold text-neutral-900 dark:text-white">Unauthorized</h1>
      <p class="max-w-md text-sm text-neutral-600 dark:text-neutral-400">
        You do not have permission to view this area. Switch accounts or return to your dashboard.
      </p>
      <a routerLink="/app">
        <app-button variant="primary">Go to workspace</app-button>
      </a>
    </main>
  `,
})
export class UnauthorizedComponent {}
