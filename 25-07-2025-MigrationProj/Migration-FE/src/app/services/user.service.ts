import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UserDto, CreateUserDto, UpdateUserDto } from '../../../../Migration-FE/src/app/models/user.model';
import { ApiResponse } from '../models/api-response.model';
import { environment } from '../../environments/environment.prod';

@Injectable({ providedIn: 'root' })
export class UserService {
  private apiUrl = environment.apiUrl + '/users';

  constructor(private http: HttpClient) {}

  getAll(): Observable<ApiResponse<UserDto[]>> {
    return this.http.get<ApiResponse<UserDto[]>>(this.apiUrl);
  }

  // Get all users with pagination
  getAllPaginated(params: any): Observable<ApiResponse<any>> {
    return this.http.get<ApiResponse<any>>(`${this.apiUrl}/paginated`, { params });
  }

  // Get user by ID
  getById(id: number): Observable<ApiResponse<UserDto>> {
    return this.http.get<ApiResponse<UserDto>>(`${this.apiUrl}/${id}`);
  }

  // Get user by email
  getByEmail(email: string): Observable<ApiResponse<UserDto>> {
    return this.http.get<ApiResponse<UserDto>>(`${this.apiUrl}/email`, { params: { email } });
  }

  // Update user
  update(id: number, user: UpdateUserDto): Observable<ApiResponse<UserDto>> {
    return this.http.put<ApiResponse<UserDto>>(`${this.apiUrl}/${id}`, user);
  }

  // Update user profile
  updateProfile(id: number, profile: any): Observable<ApiResponse<boolean>> {
    return this.http.put<ApiResponse<boolean>>(`${this.apiUrl}/profile/${id}`, profile);
  }

  // Change password
  changePassword(id: number, passwordDto: any): Observable<ApiResponse<boolean>> {
    return this.http.put<ApiResponse<boolean>>(`${this.apiUrl}/change-password/${id}`, passwordDto);
  }

  // Check if email exists
  checkEmailExists(email: string): Observable<ApiResponse<boolean>> {
    return this.http.get<ApiResponse<boolean>>(`${this.apiUrl}/check-email`, { params: { email } });
  }

  // Delete user
  delete(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.apiUrl}/${id}`);
  }
}
