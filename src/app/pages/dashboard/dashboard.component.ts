import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { DashboardService } from '../../services/dashboard.service';
import { AuthService } from '../../services/auth.service';
import { DashboardStats } from '../../models/task.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="min-h-screen bg-gray-50">
      <!-- Header -->
      <div class="bg-white shadow">
        <div class="max-w-7xl mx-auto px-4 py-6 flex justify-between items-center">
          <h1 class="text-3xl font-bold text-gray-800">Dashboard</h1>
          <div class="flex items-center space-x-4">
            <span class="text-gray-700">Welcome, <strong>{{ currentUser?.name }}</strong></span>
            <button (click)="logout()" class="px-4 py-2 bg-red-600 hover:bg-red-700 text-white rounded-lg">Logout</button>
          </div>
        </div>
      </div>

      <!-- Navigation -->
      <div class="bg-white border-b">
        <div class="max-w-7xl mx-auto px-4 flex space-x-6">
          <a routerLink="/dashboard" routerLinkActive="border-b-2 border-blue-600" class="py-4 px-2 font-semibold text-gray-700 hover:text-blue-600">
            Dashboard
          </a>
          <a routerLink="/projects" routerLinkActive="border-b-2 border-blue-600" class="py-4 px-2 font-semibold text-gray-700 hover:text-blue-600">
            Projects
          </a>
          <a routerLink="/tasks" routerLinkActive="border-b-2 border-blue-600" class="py-4 px-2 font-semibold text-gray-700 hover:text-blue-600">
            Tasks
          </a>
        </div>
      </div>

      <!-- Main Content -->
      <div class="max-w-7xl mx-auto px-4 py-8">
        <!-- Stats Cards -->
        <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
          <!-- Total Tasks -->
          <div class="bg-white rounded-lg shadow p-6">
            <div class="flex justify-between items-start">
              <div>
                <p class="text-gray-500 text-sm font-semibold">Total Tasks</p>
                <p class="text-3xl font-bold text-gray-800 mt-2">{{ stats?.totalTasks || 0 }}</p>
              </div>
              <div class="bg-blue-100 p-3 rounded-lg">
                <span class="text-blue-600 text-2xl">📋</span>
              </div>
            </div>
          </div>

          <!-- Completed Tasks -->
          <div class="bg-white rounded-lg shadow p-6">
            <div class="flex justify-between items-start">
              <div>
                <p class="text-gray-500 text-sm font-semibold">Completed</p>
                <p class="text-3xl font-bold text-green-600 mt-2">{{ stats?.completedTasks || 0 }}</p>
              </div>
              <div class="bg-green-100 p-3 rounded-lg">
                <span class="text-green-600 text-2xl">✅</span>
              </div>
            </div>
          </div>

          <!-- Pending Tasks -->
          <div class="bg-white rounded-lg shadow p-6">
            <div class="flex justify-between items-start">
              <div>
                <p class="text-gray-500 text-sm font-semibold">Pending</p>
                <p class="text-3xl font-bold text-yellow-600 mt-2">{{ stats?.pendingTasks || 0 }}</p>
              </div>
              <div class="bg-yellow-100 p-3 rounded-lg">
                <span class="text-yellow-600 text-2xl">⏳</span>
              </div>
            </div>
          </div>

          <!-- Overdue Tasks -->
          <div class="bg-white rounded-lg shadow p-6">
            <div class="flex justify-between items-start">
              <div>
                <p class="text-gray-500 text-sm font-semibold">Overdue</p>
                <p class="text-3xl font-bold text-red-600 mt-2">{{ stats?.overdueTasks || 0 }}</p>
              </div>
              <div class="bg-red-100 p-3 rounded-lg">
                <span class="text-red-600 text-2xl">🔴</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Loading State -->
        <div *ngIf="loading" class="text-center py-8">
          <p class="text-gray-600">Loading dashboard...</p>
        </div>

        <!-- Error State -->
        <div *ngIf="error && !loading" class="p-4 bg-red-100 text-red-700 rounded-lg">
          {{ error }}
        </div>

        <!-- Info -->
        <div class="bg-blue-50 border border-blue-200 rounded-lg p-6">
          <h2 class="font-semibold text-gray-800 mb-2">Getting Started</h2>
          <ul class="text-gray-700 space-y-1">
            <li>• View your projects and tasks</li>
            <li>• {{ currentUser?.role === 'Admin' ? 'Create new projects and assign tasks to team members' : 'Update your assigned tasks' }}</li>
            <li>• Track task progress and deadlines</li>
          </ul>
        </div>
      </div>
    </div>
  `,
  styles: []
})
export class DashboardComponent implements OnInit {
  stats: DashboardStats | null = null;
  currentUser: any;
  loading = true;
  error: string | null = null;

  constructor(
    private dashboardService: DashboardService,
    private authService: AuthService,
    private cdr: ChangeDetectorRef
  ) {
    this.currentUser = this.authService.getCurrentUser();
  }

  ngOnInit(): void {
    this.loadStats();
  }

  loadStats(): void {
    this.loading = true;
    this.cdr.detectChanges();
    this.dashboardService.getDashboardStats().subscribe({
      next: (data) => {
        this.stats = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.error = 'Failed to load dashboard stats';
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  logout(): void {
    this.authService.logout();
    window.location.href = '/login';
  }
}
