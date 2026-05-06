import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { TaskItem, CreateTaskRequest, UpdateTaskStatusRequest } from '../models/task.model';

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  constructor(private apiService: ApiService) { }

  getTasksByProject(projectId: string): Observable<TaskItem[]> {
    return new Observable(observer => {
      this.apiService.get<TaskItem[]>(`/api/tasks/project/${projectId}`)
        .then(response => {
          observer.next(response.data);
          observer.complete();
        })
        .catch(error => {
          observer.error(error.response?.data || error.message);
        });
    });
  }

  getTaskById(id: string): Observable<TaskItem> {
    return new Observable(observer => {
      this.apiService.get<TaskItem>(`/api/tasks/${id}`)
        .then(response => {
          observer.next(response.data);
          observer.complete();
        })
        .catch(error => {
          observer.error(error.response?.data || error.message);
        });
    });
  }

  createTask(data: CreateTaskRequest): Observable<TaskItem> {
    return new Observable(observer => {
      this.apiService.post<TaskItem>('/api/tasks', data)
        .then(response => {
          observer.next(response.data);
          observer.complete();
        })
        .catch(error => {
          observer.error(error.response?.data || error.message);
        });
    });
  }

  updateTaskStatus(id: string, data: UpdateTaskStatusRequest): Observable<TaskItem> {
    return new Observable(observer => {
      this.apiService.patch<TaskItem>(`/api/tasks/${id}/status`, data)
        .then(response => {
          observer.next(response.data);
          observer.complete();
        })
        .catch(error => {
          observer.error(error.response?.data || error.message);
        });
    });
  }

  getMyTasks(): Observable<TaskItem[]> {
    return new Observable(observer => {
      this.apiService.get<TaskItem[]>('/api/tasks/assigned-to-me')
        .then(response => {
          observer.next(response.data);
          observer.complete();
        })
        .catch(error => {
          observer.error(error.response?.data || error.message);
        });
    });
  }

  getAllTasks(): Observable<TaskItem[]> {
    return new Observable(observer => {
      this.apiService.get<TaskItem[]>('/api/tasks')
        .then(response => {
          observer.next(response.data);
          observer.complete();
        })
        .catch(error => {
          observer.error(error.response?.data || error.message);
        });
    });
  }
}
