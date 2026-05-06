export interface User {
  id: string;
  name: string;
  email: string;
  role: 'Admin' | 'Member';
  createdAt: string;
}

export interface AuthResponse {
  token: string;
  user: User;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface SignupRequest {
  name: string;
  email: string;
  password: string;
}
