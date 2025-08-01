import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ModelDto, CreateModelDto, UpdateModelDto } from '../../../../Migration-FE/src/app/models/model.model';
import { ApiResponse } from '../models/api-response.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ModelService {
  private apiUrl = environment.apiUrl + '/models';

  constructor(private http: HttpClient) {}

  getAll(): Observable<ApiResponse<ModelDto[]>> {
    return this.http.get<ApiResponse<ModelDto[]>>(this.apiUrl);
  }

  // Get all models with pagination
  getAllPaginated(params: any): Observable<ApiResponse<any>> {
    return this.http.get<ApiResponse<any>>(`${this.apiUrl}/paginated`, { params });
  }

  // Get model by ID
  getById(id: number): Observable<ApiResponse<ModelDto>> {
    return this.http.get<ApiResponse<ModelDto>>(`${this.apiUrl}/${id}`);
  }

  // Create a new model
  create(model: CreateModelDto): Observable<ApiResponse<ModelDto>> {
    return this.http.post<ApiResponse<ModelDto>>(this.apiUrl, model);
  }

  // Update an existing model
  update(id: number, model: UpdateModelDto): Observable<ApiResponse<ModelDto>> {
    return this.http.put<ApiResponse<ModelDto>>(`${this.apiUrl}/${id}`, model);
  }

  // Delete a model
  delete(id: number): Observable<ApiResponse<any>> {
    return this.http.delete<ApiResponse<any>>(`${this.apiUrl}/${id}`);
  }

  // Check if model name exists
  checkName(name: string, excludeId?: number): Observable<ApiResponse<any>> {
    const params: any = { name };
    if (excludeId !== undefined) params.excludeId = excludeId;
    return this.http.get<ApiResponse<any>>(`${this.apiUrl}/check-name`, { params });
  }
}
