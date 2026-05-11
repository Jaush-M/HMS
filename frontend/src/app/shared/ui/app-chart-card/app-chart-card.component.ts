import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { BaseChartDirective } from 'ng2-charts';
import type { ChartConfiguration } from 'chart.js';

@Component({
  selector: 'app-chart-card',
  standalone: true,
  imports: [BaseChartDirective],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <section
      class="rounded-xl border border-neutral-200/80 bg-white p-4 shadow-sm dark:border-neutral-800 dark:bg-neutral-900"
    >
      <h3 class="mb-4 text-sm font-semibold text-neutral-900 dark:text-neutral-50">
        {{ title() }}
      </h3>
      <div class="h-64">
        <canvas baseChart [type]="type()" [data]="data()" [options]="options()"></canvas>
      </div>
    </section>
  `,
})
export class AppChartCardComponent {
  title = input.required<string>();
  type = input<ChartConfiguration['type']>('line');
  data = input.required<ChartConfiguration['data']>();
  options = input<ChartConfiguration['options']>({ responsive: true, maintainAspectRatio: false });
}
