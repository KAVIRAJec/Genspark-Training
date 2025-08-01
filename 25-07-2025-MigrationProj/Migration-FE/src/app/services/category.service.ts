import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CategoryDto, CreateCategoryDto, UpdateCategoryDto } from '../../../../Migration-FE/src/app/models/category.model';
import { ApiResponse } from '../models/api-response.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class CategoryService {
  private apiUrl = environment.apiUrl + '/categories';

  constructor(private http: HttpClient) {}


  getAll(): Observable<ApiResponse<CategoryDto[]>> {
    return this.http.get<ApiResponse<CategoryDto[]>>(this.apiUrl);
  }

  getById(id: number): Observable<ApiResponse<CategoryDto>> {
    return this.http.get<ApiResponse<CategoryDto>>(`${this.apiUrl}/${id}`);
  }

  create(category: CreateCategoryDto): Observable<ApiResponse<CategoryDto>> {
    return this.http.post<ApiResponse<CategoryDto>>(this.apiUrl, category);
  }

  update(id: number, category: UpdateCategoryDto): Observable<ApiResponse<CategoryDto>> {
    return this.http.put<ApiResponse<CategoryDto>>(`${this.apiUrl}/${id}`, category);
  }

  delete(id: number): Observable<ApiResponse<void>> {
    return this.http.delete<ApiResponse<void>>(`${this.apiUrl}/${id}`);
  }

  // Get all categories with pagination
  getAllPaginated(params: any): Observable<ApiResponse<CategoryDto[]>> {
    return this.http.get<ApiResponse<CategoryDto[]>>(`${this.apiUrl}/paginated`, { params });
  }

  // Check if category name exists
  checkName(name: string, excludeId?: number): Observable<ApiResponse<boolean>> {
    const params: any = { name };
    if (excludeId !== undefined) params.excludeId = excludeId;
    return this.http.get<ApiResponse<boolean>>(`${this.apiUrl}/check-name`, { params });
  }
}
