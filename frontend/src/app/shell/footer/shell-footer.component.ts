import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
  selector: 'app-shell-footer',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <footer
      class="border-t border-neutral-200/80 bg-white/80 px-4 py-3 text-center text-xs text-neutral-500 dark:border-neutral-800 dark:bg-neutral-950/80 dark:text-neutral-400"
    >
      © {{ year }} Grand Plaza Hotel Management System
    </footer>
  `,
})
export class ShellFooterComponent {
  readonly year = new Date().getFullYear();
}
