import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProjectService } from '../../services/project.service';
import { TaskService } from '../../services/task.service';
import { AuthService } from '../../services/auth.service';
import { Project } from '../../models/project.model';
import { TaskItem } from '../../models/task.model';

@Component({
  selector: 'app-projects',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule],
  template: `
    <div class="min-h-screen bg-gray-50">
      <!-- Header -->
      <div class="bg-white shadow">
        <div class="max-w-7xl mx-auto px-4 py-6 flex justify-between items-center">
          <h1 class="text-3xl font-bold text-gray-800">Projects</h1>
          <button 
            *ngIf="isAdmin"
            (click)="toggleCreateForm()"
            class="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg"
          >
            {{ showCreateForm ? 'Cancel' : '+ New Project' }}
          </button>
        </div>
      </div>

      <!-- Navigation -->
      <div class="bg-white border-b">
        <div class="max-w-7xl mx-auto px-4 flex space-x-6">
          <a routerLink="/dashboard" class="py-4 px-2 font-semibold text-gray-700 hover:text-blue-600">Dashboard</a>
          <a routerLink="/projects" routerLinkActive="border-b-2 border-blue-600" class="py-4 px-2 font-semibold">Projects</a>
          <a routerLink="/tasks" class="py-4 px-2 font-semibold text-gray-700 hover:text-blue-600">Tasks</a>
        </div>
      </div>

      <!-- Main Content -->
      <div class="max-w-7xl mx-auto px-4 py-8">
        <!-- Create Project Form -->
        <div *ngIf="showCreateForm && isAdmin" class="bg-white rounded-lg shadow p-6 mb-8">
          <h2 class="text-xl font-bold text-gray-800 mb-4">Create New Project</h2>
          <form [formGroup]="createForm" (ngSubmit)="createProject()" class="space-y-4">
            <div>
              <label class="block text-gray-700 font-medium mb-2">Project Name</label>
              <input
                type="text"
                formControlName="name"
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                placeholder="Enter project name"
              />
            </div>
            <div>
              <label class="block text-gray-700 font-medium mb-2">Description</label>
              <textarea
                formControlName="description"
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                placeholder="Enter project description"
                rows="3"
              ></textarea>
            </div>
            <button
              type="submit"
              [disabled]="loading"
              class="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg disabled:opacity-50"
            >
              {{ loading ? 'Creating...' : 'Create Project' }}
            </button>
          </form>
        </div>

        <!-- Error Message -->
        <div *ngIf="error" class="p-4 bg-red-100 text-red-700 rounded-lg mb-6">
          {{ error }}
        </div>

        <!-- Loading State -->
        <div *ngIf="projectsLoading" class="text-center py-8">
          <p class="text-gray-600">Loading projects...</p>
        </div>

        <!-- Projects List -->
        <div *ngIf="!projectsLoading && projects.length > 0" class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div *ngFor="let project of projects" class="bg-white rounded-lg shadow p-6">
            <h3 class="text-xl font-bold text-gray-800 mb-2">{{ project.name }}</h3>
            <p class="text-gray-600 text-sm mb-4">{{ project.description || 'No description' }}</p>
            <div class="flex space-x-2">
              <button 
                (click)="openTasksModal(project)"
                class="flex-1 px-4 py-2 bg-blue-100 hover:bg-blue-200 text-blue-600 rounded-lg text-center text-sm font-semibold"
              >
                View Tasks
              </button>
              <button
                *ngIf="isAdmin"
                (click)="editProject(project)"
                class="flex-1 px-4 py-2 bg-gray-100 hover:bg-gray-200 text-gray-700 rounded-lg text-sm font-semibold"
              >
                Add Members
              </button>
            </div>
          </div>
        </div>

        <!-- Empty State -->
        <div *ngIf="!projectsLoading && projects.length === 0" class="text-center py-12">
          <p class="text-gray-600 text-lg mb-4">No projects yet</p>
          <p *ngIf="isAdmin" class="text-gray-500">Create your first project to get started!</p>
          <p *ngIf="!isAdmin" class="text-gray-500">You don't have access to any projects yet.</p>
        </div>
      </div>

      <!-- Tasks Modal -->
      <div *ngIf="showTasksModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
        <div class="bg-white rounded-lg shadow-xl max-w-2xl w-full max-h-[80vh] overflow-y-auto">
          <!-- Modal Header -->
          <div class="sticky top-0 bg-white border-b px-6 py-4 flex justify-between items-center">
            <div>
              <h2 class="text-2xl font-bold text-gray-800">{{ selectedProject?.name }}</h2>
              <p class="text-gray-600 text-sm mt-1">{{ projectTasks.length }} tasks</p>
            </div>
            <button 
              (click)="closeTasksModal()"
              class="text-gray-500 hover:text-gray-700 font-bold text-2xl"
            >
              ✕
            </button>
          </div>

          <!-- Modal Content -->
          <div class="p-6">
            <!-- Loading State -->
            <div *ngIf="tasksLoadingInModal" class="text-center py-8">
              <p class="text-gray-600">Loading tasks...</p>
            </div>

            <!-- Tasks List -->
            <div *ngIf="!tasksLoadingInModal && projectTasks.length > 0" class="space-y-3">
              <div *ngFor="let task of projectTasks" class="bg-gray-50 rounded-lg p-4 border-l-4" [ngClass]="getStatusBorderClass(task.status)">
                <div class="flex justify-between items-start mb-2">
                  <h4 class="font-semibold text-gray-800">{{ task.title }}</h4>
                  <span [ngClass]="'px-3 py-1 rounded-full text-xs font-semibold ' + getStatusClass(task.status)">
                    {{ task.status }}
                  </span>
                </div>
                <p class="text-gray-600 text-sm mb-2">{{ task.description || 'No description' }}</p>
                <div class="flex justify-between text-xs text-gray-500">
                  <span *ngIf="task.dueDate">Due: {{ task.dueDate | date:'short' }}</span>
                  <span *ngIf="task.assignedToName">Assigned to: {{ task.assignedToName }}</span>
                </div>
              </div>
            </div>

            <!-- Empty Tasks -->
            <div *ngIf="!tasksLoadingInModal && projectTasks.length === 0" class="text-center py-8">
              <p class="text-gray-600">No tasks in this project yet</p>
            </div>
          </div>

          <!-- Modal Footer -->
          <div class="border-t px-6 py-4 bg-gray-50 flex justify-end">
            <button 
              (click)="closeTasksModal()"
              class="px-4 py-2 bg-gray-300 hover:bg-gray-400 text-gray-800 rounded-lg font-semibold"
            >
              Close
            </button>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: []
})
export class ProjectsComponent implements OnInit {
  projects: Project[] = [];
  createForm: FormGroup;
  showCreateForm = false;
  projectsLoading = true;
  loading = false;
  error: string | null = null;
  isAdmin = false;

  // Modal properties
  showTasksModal = false;
  selectedProject: Project | null = null;
  projectTasks: TaskItem[] = [];
  tasksLoadingInModal = false;

  constructor(
    private projectService: ProjectService,
    private taskService: TaskService,
    private authService: AuthService,
    private fb: FormBuilder,
    private cdr: ChangeDetectorRef
  ) {
    this.isAdmin = this.authService.isAdmin();
    this.createForm = this.fb.group({
      name: ['', Validators.required],
      description: ['']
    });
  }

  ngOnInit(): void {
    this.loadProjects();
  }

  loadProjects(): void {
    this.projectsLoading = true;
    this.projectService.getUserProjects().subscribe({
      next: (data) => {
        this.projects = data;
        this.projectsLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.error = 'Failed to load projects';
        this.projectsLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  toggleCreateForm(): void {
    this.showCreateForm = !this.showCreateForm;
  }

  createProject(): void {
    if (this.createForm.invalid) return;
    this.loading = true;
    this.projectService.createProject(this.createForm.value).subscribe({
      next: () => {
        this.createForm.reset();
        this.showCreateForm = false;
        this.loadProjects();
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.error = 'Failed to create project';
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  editProject(project: Project): void {
    alert('Add members feature coming soon!');
  }

  openTasksModal(project: Project): void {
    this.selectedProject = project;
    this.showTasksModal = true;
    this.tasksLoadingInModal = true;
    this.cdr.detectChanges();
    
    this.taskService.getTasksByProject(project.id).subscribe({
      next: (tasks) => {
        this.projectTasks = tasks;
        this.tasksLoadingInModal = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.projectTasks = [];
        this.tasksLoadingInModal = false;
        this.cdr.detectChanges();
      }
    });
  }

  closeTasksModal(): void {
    this.showTasksModal = false;
    this.selectedProject = null;
    this.projectTasks = [];
    this.cdr.detectChanges();
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

  getStatusBorderClass(status: string): string {
    switch (status) {
      case 'Completed':
        return 'border-green-500';
      case 'InProgress':
        return 'border-blue-500';
      case 'Pending':
        return 'border-yellow-500';
      default:
        return 'border-gray-300';
    }
  }
}
