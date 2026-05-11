import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { PasswordRecoveryService } from '../../../core/services/password-recovery.service';
import { AppButtonComponent } from '../../../shared/ui/app-button/app-button.component';
import { MSG } from '../../../core/i18n/ui-messages';

@Component({
  selector: 'app-forgot-password',
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
    <div class="mx-auto flex min-h-[70vh] max-w-md items-center px-4 py-12">
      <mat-card class="w-full !rounded-2xl !border !border-white/10 !bg-white/5 !shadow-2xl backdrop-blur-md">
        <mat-card-header>
          <mat-card-title class="!text-white">{{ msg.forgotTitle }}</mat-card-title>
          <mat-card-subtitle class="!text-white/70"
            >Mock flow — API endpoint not yet available on the backend</mat-card-subtitle
          >
        </mat-card-header>
        <mat-card-content class="!pt-4">
          @if (!done()) {
            <form class="space-y-4" [formGroup]="form" (ngSubmit)="submit()">
              <mat-form-field appearance="outline" class="w-full">
                <mat-label>Email</mat-label>
                <input matInput type="email" formControlName="email" autocomplete="email" />
              </mat-form-field>
              <app-button variant="primary" [loading]="loading()" type="submit">Send reset link</app-button>
            </form>
          } @else {
            <p class="text-sm text-emerald-200">
              If an account exists for {{ emailSent() }}, you will receive instructions shortly.
            </p>
          }
          <p class="mt-4 text-center text-sm text-white/70">
            <a routerLink="/login" class="underline hover:text-white">Back to sign in</a>
          </p>
        </mat-card-content>
      </mat-card>
    </div>
  `,
})
export class ForgotPasswordComponent {
  private readonly fb = inject(FormBuilder);
  private readonly recovery = inject(PasswordRecoveryService);

  readonly msg = MSG.auth;
  readonly loading = signal(false);
  readonly done = signal(false);
  readonly emailSent = signal('');

  readonly form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
  });

  submit(): void {
    if (this.form.invalid) return;
    this.loading.set(true);
    const email = this.form.controls.email.value;
    this.recovery.requestReset(email).subscribe({
      next: () => {
        this.loading.set(false);
        this.emailSent.set(email);
        this.done.set(true);
      },
      error: () => this.loading.set(false),
    });
  }
}
