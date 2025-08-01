import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { LoginDto, RegisterDto, AuthenticationResult, RefreshTokenDto } from '../models/auth.model';
import { ApiResponse } from '../models/api-response.model';
import { UserDto } from '../models/user.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private apiUrl = environment.apiUrl + '/auth';

  constructor(private http: HttpClient) {}

  login(loginDto: LoginDto): Observable<ApiResponse<AuthenticationResult>> {
    return this.http.post<ApiResponse<AuthenticationResult>>(`${this.apiUrl}/login`, loginDto).pipe(
      tap(res => this.setTokens(res.data))
    );
  }

  register(registerDto: RegisterDto): Observable<ApiResponse<AuthenticationResult>> {
    return this.http.post<ApiResponse<AuthenticationResult>>(`${this.apiUrl}/register`, registerDto).pipe(
      tap(res => this.setTokens(res.data))
    );
  }

  refreshToken(refreshTokenDto: RefreshTokenDto): Observable<ApiResponse<AuthenticationResult>> {
    return this.http.post<ApiResponse<AuthenticationResult>>(`${this.apiUrl}/refresh-token`, refreshTokenDto).pipe(
      tap(res => this.setTokens(res.data))
    );
  }

  logout(): Observable<any> {
    console.log('Logging out...');
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    return this.http.post<any>(`${this.apiUrl}/logout`, {});
  }

  validateToken(token: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/validate-token`, { token });
  }

  getCurrentUser(): Observable<ApiResponse<UserDto>> {
    return this.http.get<ApiResponse<UserDto>>(`${this.apiUrl}/me`);
  }

  private setTokens(authResult: AuthenticationResult | null | undefined) {
    if (!authResult) return;
    const accessToken = authResult.token || (authResult as any)?.accessToken;
    const refreshToken = authResult.refreshToken;
    if (accessToken) {
      localStorage.setItem('accessToken', accessToken);
    }
    if (refreshToken) {
      localStorage.setItem('refreshToken', refreshToken);
    }
  }
  isLoggedIn(): boolean {
    return !!localStorage.getItem('accessToken');
  }
}
