import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import {
  UserProfile,
  LoginCredentials,
  RegistrationCredentials,
  AuthResponse,
} from '../../models';

@Injectable({
  providedIn: 'root',
})
export class ApiService {
  private http = inject(HttpClient);

  login(data: LoginCredentials): Observable<AuthResponse> {
    return this.http.post<AuthResponse>('/api/Auth/login', data);
  }

  register(data: RegistrationCredentials): Observable<AuthResponse> {
    return this.http.post<AuthResponse>('/api/Auth/register', data);
  }

  checkEmail(email: string): Observable<boolean> {
    return this.http.get<{ exists: boolean }>(
      `/api/Auth/check-email?email=${encodeURIComponent(email)}`
    ).pipe(map((res) => res.exists));
  }

  forgotPassword(data: { email: string }): Observable<any> {
    return this.http.post('/api/Auth/forgot-password', data);
  }

  getProfile(userId: number): Observable<UserProfile> {
    return this.http.get<UserProfile>(`/api/User/profile/${userId}`);
  }

  updateProfile(
    userId: number,
    data: { fullName: string }
  ): Observable<UserProfile> {
    return this.http.put<UserProfile>(
      `/api/User/profile/${userId}`,
      data
    );
  }

  changePassword(
    userId: number,
    data: { currentPassword: string; newPassword: string }
  ): Observable<any> {
    return this.http.put(`/api/User/password/${userId}`, data);
  }
}