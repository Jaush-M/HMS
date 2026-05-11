import { ChangeDetectionStrategy, Component } from '@angular/core';

/** Styled container for Angular Material tables. */
@Component({
  selector: 'app-table',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div
      class="overflow-x-auto rounded-xl border border-neutral-200/80 bg-white shadow-sm dark:border-neutral-800 dark:bg-neutral-900"
    >
      <ng-content />
    </div>
  `,
})
export class AppTableComponent {}
