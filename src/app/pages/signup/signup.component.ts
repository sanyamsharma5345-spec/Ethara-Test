import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-signup',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  template: `
    <div class="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 flex items-center justify-center p-4">
      <div class="w-full max-w-md bg-white rounded-lg shadow-lg p-8">
        <h1 class="text-3xl font-bold text-gray-800 mb-2 text-center">Create Account</h1>
        <p class="text-gray-600 text-center mb-8">Join Team Task Manager</p>

        <form [formGroup]="signupForm" (ngSubmit)="onSubmit()" class="space-y-4">
          <!-- Error Message -->
          <div *ngIf="error" class="p-3 bg-red-100 text-red-700 rounded-md text-sm">
            {{ error }}
          </div>

          <!-- Name -->
          <div>
            <label class="block text-gray-700 font-medium mb-2">Full Name</label>
            <input
              type="text"
              formControlName="name"
              class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="John Doe"
            />
            <p *ngIf="signupForm.get('name')?.invalid && signupForm.get('name')?.touched" class="text-red-500 text-sm mt-1">
              Name is required
            </p>
          </div>

          <!-- Email -->
          <div>
            <label class="block text-gray-700 font-medium mb-2">Email</label>
            <input
              type="email"
              formControlName="email"
              class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="your@email.com"
            />
            <p *ngIf="signupForm.get('email')?.invalid && signupForm.get('email')?.touched" class="text-red-500 text-sm mt-1">
              Valid email is required
            </p>
          </div>

          <!-- Password -->
          <div>
            <label class="block text-gray-700 font-medium mb-2">Password</label>
            <input
              type="password"
              formControlName="password"
              class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="Enter password"
            />
            <p *ngIf="signupForm.get('password')?.invalid && signupForm.get('password')?.touched" class="text-red-500 text-sm mt-1">
              Password must be at least 6 characters
            </p>
          </div>

          <!-- Submit Button -->
          <button
            type="submit"
            [disabled]="loading"
            class="w-full bg-blue-600 hover:bg-blue-700 text-white font-bold py-2 rounded-lg transition disabled:opacity-50"
          >
            {{ loading ? 'Creating Account...' : 'Sign Up' }}
          </button>
        </form>

        <!-- Login Link -->
        <p class="text-center mt-6 text-gray-600">
          Already have an account? <a routerLink="/login" class="text-blue-600 hover:underline font-semibold">Sign in</a>
        </p>
      </div>
    </div>
  `,
  styles: []
})
export class SignupComponent {
  signupForm: FormGroup;
  loading = false;
  error: string | null = null;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.signupForm = this.fb.group({
      name: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  onSubmit(): void {
    if (this.signupForm.invalid) return;

    this.loading = true;
    this.error = null;

    this.authService.signup(this.signupForm.value).subscribe({
      next: () => {
        alert('Account created! Please login.');
        this.router.navigate(['/login']);
      },
      error: (err) => {
        this.error = err.message || 'Signup failed';
        this.loading = false;
      }
    });
  }
}
