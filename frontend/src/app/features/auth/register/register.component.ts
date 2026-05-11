import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { AuthService } from '../../../core/auth/auth.service';
import type { UserRole } from '../../../core/constants/roles';
import { isUserRole } from '../../../core/constants/roles';
import { AppButtonComponent } from '../../../shared/ui/app-button/app-button.component';
import { MSG } from '../../../core/i18n/ui-messages';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    AppButtonComponent,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="mx-auto flex min-h-[70vh] max-w-lg items-center px-4 py-12">
      <mat-card class="w-full !rounded-2xl !border !border-white/10 !bg-white/5 !shadow-2xl backdrop-blur-md">
        <mat-card-header>
          <mat-card-title class="!text-white">{{ msg.registerTitle }}</mat-card-title>
          <mat-card-subtitle class="!text-white/70">Guests only — staff accounts are issued by IT</mat-card-subtitle>
        </mat-card-header>
        <mat-card-content class="!pt-4">
          <form class="grid gap-4 md:grid-cols-2" [formGroup]="form" (ngSubmit)="submit()">
            <mat-form-field appearance="outline" class="w-full md:col-span-2">
              <mat-label>Email</mat-label>
              <input matInput type="email" formControlName="email" autocomplete="email" />
            </mat-form-field>
            <mat-form-field appearance="outline" class="w-full md:col-span-2">
              <mat-label>Password</mat-label>
              <input matInput type="password" formControlName="password" autocomplete="new-password" />
            </mat-form-field>
            <mat-form-field appearance="outline" class="w-full">
              <mat-label>First name</mat-label>
              <input matInput formControlName="firstName" autocomplete="given-name" />
            </mat-form-field>
            <mat-form-field appearance="outline" class="w-full">
              <mat-label>Last name</mat-label>
              <input matInput formControlName="lastName" autocomplete="family-name" />
            </mat-form-field>
            <mat-form-field appearance="outline" class="w-full md:col-span-2">
              <mat-label>Phone</mat-label>
              <input matInput formControlName="phone" autocomplete="tel" />
            </mat-form-field>
            <mat-form-field appearance="outline" class="w-full md:col-span-2">
              <mat-label>Address</mat-label>
              <textarea matInput rows="2" formControlName="address"></textarea>
            </mat-form-field>
            @if (error()) {
              <p class="md:col-span-2 text-sm text-rose-300" role="alert">{{ error() }}</p>
            }
            <div class="md:col-span-2">
              <app-button variant="primary" [loading]="loading()" type="submit">Create account</app-button>
            </div>
          </form>
          <p class="mt-4 text-center text-sm text-white/70">
            Already registered?
            <a routerLink="/login" class="underline hover:text-white">Sign in</a>
          </p>
        </mat-card-content>
      </mat-card>
    </div>
  `,
})
export class RegisterComponent {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  readonly msg = MSG.auth;
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  readonly form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8)]],
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    phone: [''],
    address: [''],
  });

  submit(): void {
    if (this.form.invalid) return;
    this.loading.set(true);
    this.error.set(null);
    const v = this.form.getRawValue();
    this.auth
      .register({
        email: v.email,
        password: v.password,
        firstName: v.firstName,
        lastName: v.lastName,
        phone: v.phone || undefined,
        address: v.address || undefined,
      })
      .subscribe({
        next: (res) => {
          this.loading.set(false);
          if (!isUserRole(res.role)) return;
          void this.router.navigateByUrl('/app/guest/dashboard');
        },
        error: (err: { error?: unknown }) => {
          this.loading.set(false);
          const body = typeof err.error === 'string' ? err.error : 'Registration failed.';
          this.error.set(body);
        },
      });
  }
}
