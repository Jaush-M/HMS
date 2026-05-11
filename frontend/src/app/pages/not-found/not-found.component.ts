import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AppButtonComponent } from '../../shared/ui/app-button/app-button.component';

@Component({
  selector: 'app-not-found',
  standalone: true,
  imports: [RouterLink, AppButtonComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <main
      class="flex min-h-screen flex-col items-center justify-center gap-4 bg-neutral-50 px-4 text-center dark:bg-neutral-950"
    >
      <p class="text-sm font-semibold uppercase tracking-widest text-blue-600 dark:text-blue-400">
        404
      </p>
      <h1 class="text-2xl font-semibold text-neutral-900 dark:text-white">Page not found</h1>
      <p class="max-w-md text-sm text-neutral-600 dark:text-neutral-400">
        The page you requested does not exist or was moved.
      </p>
      <a routerLink="/">
        <app-button variant="primary">Back home</app-button>
      </a>
    </main>
  `,
})
export class NotFoundComponent {}
