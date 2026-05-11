import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';

export interface BreadcrumbItem {
  label: string;
  link?: string | readonly unknown[];
}

@Component({
  selector: 'app-breadcrumb',
  standalone: true,
  imports: [RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <nav aria-label="Breadcrumb" class="text-sm text-neutral-500 dark:text-neutral-400">
      <ol class="flex flex-wrap items-center gap-2">
        @for (item of items(); track $index; let last = $last) {
          <li class="flex items-center gap-2">
            @if (!last && item.link) {
              <a
                class="font-medium text-neutral-600 hover:text-neutral-900 dark:text-neutral-300 dark:hover:text-white"
                [routerLink]="item.link"
                >{{ item.label }}</a
              >
            } @else {
              <span [class.font-semibold]="last" [class.text-neutral-900]="last" [class.dark:text-white]="last">
                {{ item.label }}
              </span>
            }
            @if (!last) {
              <span aria-hidden="true" class="text-neutral-400">/</span>
            }
          </li>
        }
      </ol>
    </nav>
  `,
})
export class AppBreadcrumbComponent {
  items = input<BreadcrumbItem[]>([]);
}
