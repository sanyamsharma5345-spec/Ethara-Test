import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Project, CreateProjectRequest, AddMemberRequest } from '../models/project.model';

@Injectable({
  providedIn: 'root'
})
export class ProjectService {
  constructor(private apiService: ApiService) { }

  getUserProjects(): Observable<Project[]> {
    return new Observable(observer => {
      this.apiService.get<Project[]>('/api/projects')
        .then(response => {
          observer.next(response.data);
          observer.complete();
        })
        .catch(error => {
          observer.error(error.response?.data || error.message);
        });
    });
  }

  getProjectById(id: string): Observable<Project> {
    return new Observable(observer => {
      this.apiService.get<Project>(`/api/projects/${id}`)
        .then(response => {
          observer.next(response.data);
          observer.complete();
        })
        .catch(error => {
          observer.error(error.response?.data || error.message);
        });
    });
  }

  createProject(data: CreateProjectRequest): Observable<Project> {
    return new Observable(observer => {
      this.apiService.post<Project>('/api/projects', data)
        .then(response => {
          observer.next(response.data);
          observer.complete();
        })
        .catch(error => {
          observer.error(error.response?.data || error.message);
        });
    });
  }

  addMember(projectId: string, userId: string): Observable<any> {
    return new Observable(observer => {
      this.apiService.post<any>(`/api/projects/${projectId}/members`, { userId })
        .then(response => {
          observer.next(response.data);
          observer.complete();
        })
        .catch(error => {
          observer.error(error.response?.data || error.message);
        });
    });
  }

  getProjectMembers(projectId: string): Observable<any[]> {
    return new Observable(observer => {
      this.apiService.get<any[]>(`/api/projects/${projectId}/members`)
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
