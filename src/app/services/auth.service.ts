import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { ApiService } from './api.service';
import { AuthResponse, LoginRequest, SignupRequest, User } from '../models/auth.model';
import { isPlatformBrowser } from '@angular/common';
import { PLATFORM_ID, inject } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private platformId = inject(PLATFORM_ID);
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  private isAuthenticatedSubject = new BehaviorSubject<boolean>(this.hasToken());
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  constructor(private apiService: ApiService) {
    this.loadCurrentUser();
  }

  private hasToken(): boolean {
    if (!isPlatformBrowser(this.platformId)) return false;
    return !!localStorage.getItem('token');
  }

  private loadCurrentUser(): void {
    if (!isPlatformBrowser(this.platformId)) return;
    const userStr = localStorage.getItem('user');
    if (userStr) {
      try {
        const user = JSON.parse(userStr);
        this.currentUserSubject.next(user);
      } catch (e) {
        localStorage.removeItem('user');
      }
    }
  }

  login(credentials: LoginRequest): Observable<any> {
    return new Observable(observer => {
      this.apiService.post<AuthResponse>('/api/auth/login', credentials)
        .then(response => {
          const { token, user } = response.data;
          if (isPlatformBrowser(this.platformId)) {
            localStorage.setItem('token', token);
            localStorage.setItem('user', JSON.stringify(user));
          }
          this.currentUserSubject.next(user);
          this.isAuthenticatedSubject.next(true);
          observer.next(response.data);
          observer.complete();
        })
        .catch(error => {
          observer.error(error.response?.data || error.message);
        });
    });
  }

  signup(data: SignupRequest): Observable<any> {
    return new Observable(observer => {
      this.apiService.post<AuthResponse>('/api/auth/register', data)
        .then(response => {
          const { token, user } = response.data;
          if (isPlatformBrowser(this.platformId)) {
            localStorage.setItem('token', token);
            localStorage.setItem('user', JSON.stringify(user));
          }
          this.currentUserSubject.next(user);
          this.isAuthenticatedSubject.next(true);
          observer.next(response.data);
          observer.complete();
        })
        .catch(error => {
          observer.error(error.response?.data || error.message);
        });
    });
  }

  logout(): void {
    if (isPlatformBrowser(this.platformId)) {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
    }
    this.currentUserSubject.next(null);
    this.isAuthenticatedSubject.next(false);
  }

  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }

  isAuthenticated(): boolean {
    return this.isAuthenticatedSubject.value;
  }

  isAdmin(): boolean {
    const user = this.getCurrentUser();
    return user?.role === 'Admin';
  }
}
