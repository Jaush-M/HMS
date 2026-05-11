import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

export interface SidebarNavItem {
  label: string;
  icon: string;
  link: readonly unknown[];
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <aside
      class="flex h-full flex-col border-r border-neutral-200/80 bg-white/90 backdrop-blur dark:border-neutral-800 dark:bg-neutral-950/90"
      [class.w-64]="!collapsed()"
      [class.w-[4.5rem]]="collapsed()"
    >
      <div class="flex h-14 items-center gap-2 border-b border-neutral-200/80 px-4 dark:border-neutral-800">
        <span
          class="material-icons-outlined text-blue-600 dark:text-blue-400"
          aria-hidden="true"
          >apartment</span
        >
        @if (!collapsed()) {
          <span class="text-sm font-semibold tracking-tight text-neutral-900 dark:text-white"
            >Grand Plaza</span
          >
        }
      </div>
      <nav class="flex-1 space-y-1 overflow-y-auto p-2" aria-label="Primary">
        @for (item of items(); track item.link.join('/')) {
          <a
            class="flex items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium text-neutral-600 hover:bg-neutral-100 hover:text-neutral-900 dark:text-neutral-300 dark:hover:bg-neutral-800 dark:hover:text-white"
            [routerLink]="item.link"
            routerLinkActive="bg-blue-50 text-blue-800 dark:bg-blue-950/50 dark:text-blue-200"
            [routerLinkActiveOptions]="{ exact: item.link.length <= 1 }"
          >
            <span class="material-icons-outlined text-[20px]" aria-hidden="true">{{ item.icon }}</span>
            @if (!collapsed()) {
              <span>{{ item.label }}</span>
            }
          </a>
        }
      </nav>
      <button
        type="button"
        class="m-2 flex items-center justify-center gap-2 rounded-lg border border-neutral-200 px-2 py-2 text-xs font-medium text-neutral-600 hover:bg-neutral-50 dark:border-neutral-700 dark:text-neutral-300 dark:hover:bg-neutral-800"
        (click)="toggleCollapse.emit()"
        [attr.aria-expanded]="!collapsed()"
        [attr.aria-label]="collapsed() ? 'Expand sidebar' : 'Collapse sidebar'"
      >
        <span class="material-icons-outlined text-[18px]" aria-hidden="true">
          {{ collapsed() ? 'chevron_right' : 'chevron_left' }}
        </span>
        @if (!collapsed()) {
          <span>Collapse</span>
        }
      </button>
    </aside>
  `,
})
export class AppSidebarComponent {
  items = input<SidebarNavItem[]>([]);
  collapsed = input(false);
  toggleCollapse = output<void>();
}
