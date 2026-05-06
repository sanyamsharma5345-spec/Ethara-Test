import { Injectable } from '@angular/core';
import axios, { AxiosInstance } from 'axios';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private api: AxiosInstance;

  constructor() {
    this.api = axios.create({
      baseURL: environment.apiUrl,
      headers: {
        'Content-Type': 'application/json'
      }
    });

    // Add authorization header interceptor
    this.api.interceptors.request.use((config) => {
      const token = localStorage.getItem('token');
      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      }
      return config;
    });
  }

  get<T>(url: string) {
    return this.api.get<T>(url);
  }

  post<T>(url: string, data?: any) {
    return this.api.post<T>(url, data);
  }

  put<T>(url: string, data?: any) {
    return this.api.put<T>(url, data);
  }

  patch<T>(url: string, data?: any) {
    return this.api.patch<T>(url, data);
  }

  delete<T>(url: string) {
    return this.api.delete<T>(url);
  }
}
