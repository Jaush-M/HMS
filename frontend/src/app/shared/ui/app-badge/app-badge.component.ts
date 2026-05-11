import { NgClass } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';

@Component({
  selector: 'app-badge',
  standalone: true,
  imports: [NgClass],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <span
      class="inline-flex items-center rounded-full px-2 py-0.5 text-xs font-medium"
      [ngClass]="palette()"
    >
      <ng-content />
    </span>
  `,
})
export class AppBadgeComponent {
  tone = input<'neutral' | 'success' | 'warning' | 'danger' | 'info'>('neutral');

  readonly palette = computed(() => {
    switch (this.tone()) {
      case 'success':
        return 'bg-emerald-100 text-emerald-800 dark:bg-emerald-900/40 dark:text-emerald-200';
      case 'warning':
        return 'bg-amber-100 text-amber-900 dark:bg-amber-900/40 dark:text-amber-100';
      case 'danger':
        return 'bg-red-100 text-red-800 dark:bg-red-900/40 dark:text-red-200';
      case 'info':
        return 'bg-blue-100 text-blue-800 dark:bg-blue-900/40 dark:text-blue-200';
      default:
        return 'bg-neutral-100 text-neutral-700 dark:bg-neutral-800 dark:text-neutral-200';
    }
  });
}
