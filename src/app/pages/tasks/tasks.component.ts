import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TaskService } from '../../services/task.service';
import { ProjectService } from '../../services/project.service';
import { AuthService } from '../../services/auth.service';
import { TaskItem } from '../../models/task.model';
import { Project } from '../../models/project.model';

@Component({
  selector: 'app-tasks',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule],
  template: `
    <div class="min-h-screen bg-gray-50">
      <!-- Header -->
      <div class="bg-white shadow">
        <div class="max-w-7xl mx-auto px-4 py-6 flex justify-between items-center">
          <h1 class="text-3xl font-bold text-gray-800">Tasks</h1>
          <button 
            *ngIf="isAdmin"
            (click)="toggleCreateForm()"
            class="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg"
          >
            {{ showCreateForm ? 'Cancel' : '+ New Task' }}
          </button>
        </div>
      </div>

      <!-- Navigation -->
      <div class="bg-white border-b">
        <div class="max-w-7xl mx-auto px-4 flex space-x-6">
          <a routerLink="/dashboard" class="py-4 px-2 font-semibold text-gray-700 hover:text-blue-600">Dashboard</a>
          <a routerLink="/projects" class="py-4 px-2 font-semibold text-gray-700 hover:text-blue-600">Projects</a>
          <a routerLink="/tasks" routerLinkActive="border-b-2 border-blue-600" class="py-4 px-2 font-semibold">Tasks</a>
        </div>
      </div>

      <!-- Main Content -->
      <div class="max-w-7xl mx-auto px-4 py-8">
        <!-- Project Context -->
        <div *ngIf="selectedProjectId" class="mb-6 p-4 bg-blue-50 border-l-4 border-blue-500 rounded">
          <p class="text-gray-700"><strong>Viewing tasks for project:</strong> <span class="text-blue-600">{{ getProjectName() }}</span></p>
          <a routerLink="/projects" class="text-sm text-blue-600 hover:underline mt-2 inline-block">← Back to Projects</a>
        </div>

        <!-- Create Task Form -->
        <div *ngIf="showCreateForm && isAdmin" class="bg-white rounded-lg shadow p-6 mb-8">
          <h2 class="text-xl font-bold text-gray-800 mb-4">Create New Task</h2>
          <form [formGroup]="createForm" (ngSubmit)="createTask()" class="space-y-4">
            <div>
              <label class="block text-gray-700 font-medium mb-2">Task Title</label>
              <input
                type="text"
                formControlName="title"
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                placeholder="Enter task title"
              />
            </div>
            <div>
              <label class="block text-gray-700 font-medium mb-2">Description</label>
              <textarea
                formControlName="description"
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                placeholder="Enter task description"
                rows="3"
              ></textarea>
            </div>
            <div>
              <label class="block text-gray-700 font-medium mb-2">Due Date</label>
              <input
                type="date"
                formControlName="dueDate"
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
            <div>
              <label class="block text-gray-700 font-medium mb-2">Assign To</label>
              <input
                type="text"
                formControlName="assignedToId"
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                placeholder="User ID (optional)"
              />
            </div>
            <div>
              <label class="block text-gray-700 font-medium mb-2">Project</label>
              <select
                formControlName="projectId"
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
                <option value="">Select a project</option>
                <option *ngFor="let project of projects" [value]="project.id">{{ project.name }}</option>
              </select>
            </div>
            <button
              type="submit"
              [disabled]="loading"
              class="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg disabled:opacity-50"
            >
              {{ loading ? 'Creating...' : 'Create Task' }}
            </button>
          </form>
        </div>

        <!-- Error Message -->
        <div *ngIf="error" class="p-4 bg-red-100 text-red-700 rounded-lg mb-6">
          {{ error }}
        </div>

        <!-- Filter Buttons -->
        <div class="flex space-x-4 mb-6">
          <button
            (click)="filterStatus = 'All'"
            [class]="'px-4 py-2 rounded-lg font-semibold ' + (filterStatus === 'All' ? 'bg-blue-600 text-white' : 'bg-white text-gray-700 border')"
          >
            All
          </button>
          <button
            (click)="filterStatus = 'Pending'"
            [class]="'px-4 py-2 rounded-lg font-semibold ' + (filterStatus === 'Pending' ? 'bg-yellow-600 text-white' : 'bg-white text-gray-700 border')"
          >
            Pending
          </button>
          <button
            (click)="filterStatus = 'InProgress'"
            [class]="'px-4 py-2 rounded-lg font-semibold ' + (filterStatus === 'InProgress' ? 'bg-blue-600 text-white' : 'bg-white text-gray-700 border')"
          >
            In Progress
          </button>
          <button
            (click)="filterStatus = 'Completed'"
            [class]="'px-4 py-2 rounded-lg font-semibold ' + (filterStatus === 'Completed' ? 'bg-green-600 text-white' : 'bg-white text-gray-700 border')"
          >
            Completed
          </button>
        </div>

        <!-- Loading State -->
        <div *ngIf="tasksLoading" class="text-center py-8">
          <p class="text-gray-600">Loading tasks...</p>
        </div>

        <!-- Tasks List -->
        <div *ngIf="!tasksLoading && getFilteredTasks().length > 0" class="space-y-4">
          <div *ngFor="let task of getFilteredTasks()" class="bg-white rounded-lg shadow p-6">
            <div class="flex justify-between items-start mb-3">
              <div>
                <h3 class="text-lg font-bold text-gray-800">{{ task.title }}</h3>
                <p class="text-gray-600 text-sm mt-1">{{ task.description || 'No description' }}</p>
              </div>
              <span
                [class]="'px-3 py-1 rounded-full text-sm font-semibold ' + getStatusClass(task.status)"
              >
                {{ task.status }}
              </span>
            </div>

            <div class="flex justify-between items-center text-sm text-gray-500 mb-4">
              <span *ngIf="task.dueDate">Due: {{ task.dueDate | date: 'short' }}</span>
              <span *ngIf="task.isOverdue" class="text-red-600 font-semibold">⚠️ Overdue</span>
            </div>

            <div *ngIf="!isAdmin" class="flex space-x-2">
              <button
                *ngIf="task.status !== 'Completed'"
                (click)="updateTaskStatus(task, 'InProgress')"
                class="flex-1 px-3 py-2 bg-blue-100 hover:bg-blue-200 text-blue-600 rounded-lg text-sm font-semibold"
              >
                Mark In Progress
              </button>
              <button
                *ngIf="task.status !== 'Completed'"
                (click)="updateTaskStatus(task, 'Completed')"
                class="flex-1 px-3 py-2 bg-green-100 hover:bg-green-200 text-green-600 rounded-lg text-sm font-semibold"
              >
                Mark Completed
              </button>
            </div>
          </div>
        </div>

        <!-- Empty State -->
        <div *ngIf="!tasksLoading && getFilteredTasks().length === 0" class="text-center py-12">
          <p class="text-gray-600 text-lg">No {{ filterStatus !== 'All' ? filterStatus.toLowerCase() : '' }} tasks</p>
        </div>
      </div>
    </div>
  `,
  styles: []
})
export class TasksComponent implements OnInit {
  tasks: TaskItem[] = [];
  projects: Project[] = [];
  selectedProject: Project | null = null;
  createForm: FormGroup;
  showCreateForm = false;
  tasksLoading = true;
  loading = false;
  error: string | null = null;
  isAdmin = false;
  filterStatus = 'All';
  selectedProjectId: string | null = null;

  constructor(
    private taskService: TaskService,
    private projectService: ProjectService,
    private authService: AuthService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private cdr: ChangeDetectorRef
  ) {
    this.isAdmin = this.authService.isAdmin();
    this.createForm = this.fb.group({
      title: ['', Validators.required],
      description: [''],
      dueDate: [''],
      assignedToId: [''],
      projectId: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.selectedProjectId = params['projectId'] || null;
      this.cdr.detectChanges();
      
      if (this.selectedProjectId) {
        // Load specific project details
        this.projectService.getProjectById(this.selectedProjectId).subscribe({
          next: (project) => {
            this.selectedProject = project;
            this.cdr.detectChanges();
          },
          error: () => {
            this.error = 'Failed to load project details';
            this.cdr.detectChanges();
          }
        });
      }
      
      this.loadProjects();
      this.loadTasks();
    });
  }

  loadProjects(): void {
    this.projectService.getUserProjects().subscribe({
      next: (data) => {
        this.projects = data;
        this.cdr.detectChanges();
      },
      error: () => {
        this.error = 'Failed to load projects';
        this.cdr.detectChanges();
      }
    });
  }

  loadTasks(): void {
    this.tasksLoading = true;
    this.cdr.detectChanges();
    
    if (this.selectedProjectId) {
      // Load tasks for specific project
      this.taskService.getTasksByProject(this.selectedProjectId).subscribe({
        next: (data) => {
          this.tasks = data;
          this.tasksLoading = false;
          this.cdr.detectChanges();
        },
        error: (err) => {
          this.error = 'Failed to load project tasks';
          this.tasksLoading = false;
          this.cdr.detectChanges();
        }
      });
    } else {
      // Load all tasks
      this.taskService.getAllTasks().subscribe({
        next: (data) => {
          this.tasks = data;
          this.tasksLoading = false;
          this.cdr.detectChanges();
        },
        error: (err) => {
          this.error = 'Failed to load tasks';
          this.tasksLoading = false;
          this.cdr.detectChanges();
        }
      });
    }
  }

  toggleCreateForm(): void {
    this.showCreateForm = !this.showCreateForm;
    this.cdr.detectChanges();
  }

  createTask(): void {
    if (this.createForm.invalid) return;
    this.loading = true;
    this.cdr.detectChanges();
    const formValue = this.createForm.value;
    const task = {
      ...formValue,
      assignedToId: formValue.assignedToId || undefined
    };
    this.taskService.createTask(task).subscribe({
      next: () => {
        this.createForm.reset();
        this.showCreateForm = false;
        this.loading = false;
        this.cdr.detectChanges();
        this.loadTasks();
      },
      error: (err) => {
        this.error = 'Failed to create task';
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  updateTaskStatus(task: TaskItem, newStatus: string): void {
    this.taskService.updateTaskStatus(task.id, { status: newStatus as any }).subscribe({
      next: () => {
        this.cdr.detectChanges();
        this.loadTasks();
      },
      error: (err) => {
        this.error = 'Failed to update task';
        this.cdr.detectChanges();
      }
    });
  }

  getFilteredTasks(): TaskItem[] {
    if (this.filterStatus === 'All') {
      return this.tasks;
    }
    return this.tasks.filter(t => t.status === this.filterStatus);
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'Completed':
        return 'bg-green-100 text-green-800';
      case 'InProgress':
        return 'bg-blue-100 text-blue-800';
      case 'Pending':
        return 'bg-yellow-100 text-yellow-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  }

  getProjectName(): string {
    if (!this.selectedProjectId) return '';
    return this.selectedProject?.name || 'Loading...';
  }
}
