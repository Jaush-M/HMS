import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
  selector: 'app-stat-card',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <article
      class="rounded-xl border border-neutral-200/80 bg-white p-4 shadow-sm dark:border-neutral-800 dark:bg-neutral-900"
    >
      <p class="text-xs font-medium uppercase tracking-wide text-neutral-500 dark:text-neutral-400">
        {{ label() }}
      </p>
      <p class="mt-2 text-2xl font-semibold tabular-nums text-neutral-900 dark:text-neutral-50">
        {{ value() }}
      </p>
      @if (hint()) {
        <p class="mt-1 text-xs text-neutral-500 dark:text-neutral-400">{{ hint() }}</p>
      }
    </article>
  `,
})
export class AppStatCardComponent {
  label = input.required<string>();
  value = input.required<string>();
  hint = input<string>('');
}
