import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { DashboardStats } from '../models/task.model';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  constructor(private apiService: ApiService) { }

  getDashboardStats(): Observable<DashboardStats> {
    return new Observable(observer => {
      this.apiService.get<DashboardStats>('/api/dashboard')
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
