import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ColorDto, CreateColorDto, UpdateColorDto } from '../../../../Migration-FE/src/app/models/color.model';
import { ApiResponse } from '../models/api-response.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ColorService {
  private apiUrl = environment.apiUrl + '/colors';

  constructor(private http: HttpClient) {}

  getAll(): Observable<ApiResponse<ColorDto[]>> {
    return this.http.get<ApiResponse<ColorDto[]>>(this.apiUrl);
  }

  // Get all colors with pagination
  getAllPaginated(params: any): Observable<ApiResponse<ColorDto[]>> {
    return this.http.get<ApiResponse<ColorDto[]>>(`${this.apiUrl}/paginated`, { params });
  }

  // Get color by ID
  getById(id: number): Observable<ApiResponse<ColorDto>> {
    return this.http.get<ApiResponse<ColorDto>>(`${this.apiUrl}/${id}`);
  }

  // Create a new color
  create(color: CreateColorDto): Observable<ApiResponse<ColorDto>> {
    return this.http.post<ApiResponse<ColorDto>>(this.apiUrl, color);
  }

  // Update an existing color
  update(id: number, color: UpdateColorDto): Observable<ApiResponse<ColorDto>> {
    return this.http.put<ApiResponse<ColorDto>>(`${this.apiUrl}/${id}`, color);
  }

  // Delete a color
  delete(id: number): Observable<ApiResponse<void>> {
    return this.http.delete<ApiResponse<void>>(`${this.apiUrl}/${id}`);
  }

  // Check if color name exists
  checkName(name: string, excludeId?: number): Observable<any> {
    const params: any = { name };
    if (excludeId !== undefined) params.excludeId = excludeId;
    return this.http.get<any>(`${this.apiUrl}/check-name`, { params });
  }
}
