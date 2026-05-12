import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { AuthService } from '../../../core/auth/auth.service';
import type { UserRole } from '../../../core/constants/roles';
import { isUserRole } from '../../../core/constants/roles';
import { AppButtonComponent } from '../../../shared/ui/app-button/app-button.component';
import { MSG } from '../../../core/i18n/ui-messages';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatFormFieldModule,
    MatInputModule,
    MatCheckboxModule,
    AppButtonComponent,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="flex min-h-[calc(100vh-68px)]">
      <!-- Left: hero image -->
      <div class="relative hidden flex-1 lg:block">
        <img
          src="https://images.unsplash.com/photo-1574223706388-0e0f6f0390b2?h=1200&w=800&auto=format&fit=crop"
          alt=""
          aria-hidden="true"
          class="h-full w-full object-cover"
        />
        <div
          class="absolute inset-0 bg-gradient-to-br from-zinc-950/75 via-zinc-900/50 to-zinc-800/30"
        ></div>
        <div class="absolute inset-0 flex flex-col justify-end p-12">
          <img
            src="/logo.png"
            alt="Grand Plaza"
            class="mb-6 h-12 w-auto object-contain object-left"
          />
          <p class="text-lg font-medium text-white/90">Your home away from home.</p>
          <p class="mt-1 text-sm text-white/55">Luxury hospitality in the heart of the Maldives.</p>
        </div>
      </div>

      <!-- Right: form -->
      <div class="flex flex-1 items-center justify-center px-6 py-12 lg:px-12">
        <div class="w-full max-w-sm">
          <h1 class="text-2xl font-semibold tracking-tight text-zinc-900">{{ msg.loginTitle }}</h1>
          <p class="mt-1 text-sm text-zinc-500">Use your Grand Plaza credentials</p>

          <form class="mt-8 space-y-5" [formGroup]="form" (ngSubmit)="submit()">
            <mat-form-field appearance="outline" class="w-full">
              <mat-label>Email</mat-label>
              <input matInput type="email" formControlName="email" autocomplete="username" />
            </mat-form-field>
            <mat-form-field appearance="outline" class="w-full">
              <mat-label>Password</mat-label>
              <input
                matInput
                type="password"
                formControlName="password"
                autocomplete="current-password"
              />
            </mat-form-field>
            <mat-checkbox formControlName="remember">Stay signed in</mat-checkbox>
            @if (error()) {
              <p class="rounded-lg bg-rose-50 px-3 py-2 text-sm text-rose-700" role="alert">
                {{ error() }}
              </p>
            }
            <app-button variant="primary" class="w-full" [loading]="loading()" type="submit">
              Sign in
            </app-button>
          </form>

          <p class="mt-6 text-center text-sm text-zinc-500">
            <a
              routerLink="/forgot-password"
              class="font-medium text-zinc-900 underline-offset-4 hover:underline"
              >Forgot password?</a
            >
            <span class="mx-2 text-zinc-300">·</span>
            <a
              routerLink="/register"
              class="font-medium text-zinc-900 underline-offset-4 hover:underline"
              >Create account</a
            >
          </p>
        </div>
      </div>
    </div>
  `,
})
export class LoginComponent {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  readonly msg = MSG.auth;
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  readonly form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required],
    remember: [false],
  });

  submit(): void {
    if (this.form.invalid) return;
    this.loading.set(true);
    this.error.set(null);
    this.auth.login(this.form.getRawValue()).subscribe({
      next: (res) => {
        this.loading.set(false);
        if (!isUserRole(res.role)) return;
        const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl');
        if (returnUrl && returnUrl.startsWith('/')) {
          void this.router.navigateByUrl(returnUrl);
        } else {
          this.auth.navigateAfterLogin(res.role as UserRole);
        }
      },
      error: (err: { status?: number; error?: unknown }) => {
        this.loading.set(false);
        const body =
          typeof err.error === 'string'
            ? err.error
            : 'Unable to sign in. Check your email and password.';
        this.error.set(body);
      },
    });
  }
}
