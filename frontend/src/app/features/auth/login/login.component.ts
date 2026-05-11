import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
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
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatCheckboxModule,
    AppButtonComponent,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="mx-auto flex min-h-[70vh] max-w-md items-center px-4 py-12">
      <mat-card class="w-full !rounded-2xl !border !border-white/10 !bg-white/5 !shadow-2xl backdrop-blur-md">
        <mat-card-header>
          <mat-card-title class="!text-white">{{ msg.loginTitle }}</mat-card-title>
          <mat-card-subtitle class="!text-white/70">Use your Grand Plaza credentials</mat-card-subtitle>
        </mat-card-header>
        <mat-card-content class="!pt-4">
          <form class="space-y-4" [formGroup]="form" (ngSubmit)="submit()">
            <mat-form-field appearance="outline" class="w-full" color="primary">
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
            <mat-checkbox formControlName="remember" class="!text-white/80"
              >Stay signed in on this device</mat-checkbox
            >
            @if (error()) {
              <p class="text-sm text-rose-300" role="alert">{{ error() }}</p>
            }
            <app-button variant="primary" class="w-full" [loading]="loading()" type="submit">
              Sign in
            </app-button>
          </form>
          <p class="mt-4 text-center text-sm text-white/70">
            <a routerLink="/forgot-password" class="underline hover:text-white">Forgot password?</a>
            ·
            <a routerLink="/register" class="underline hover:text-white">Create account</a>
          </p>
        </mat-card-content>
      </mat-card>
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
