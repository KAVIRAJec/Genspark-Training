import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { NewsDto, CreateNewsDto, UpdateNewsDto } from '../../../../Migration-FE/src/app/models/news.model';
import { ApiResponse } from '../models/api-response.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class NewsService {
  private apiUrl = environment.apiUrl + '/news';

  constructor(private http: HttpClient) {}

  getAll(): Observable<ApiResponse<NewsDto[]>> {
    return this.http.get<ApiResponse<NewsDto[]>>(this.apiUrl);
  }

  // Get all news with pagination
  getAllPaginated(params: any): Observable<ApiResponse<any>> {
    return this.http.get<ApiResponse<any>>(`${this.apiUrl}/paginated`, { params });
  }

  // Get news by ID
  getById(id: number): Observable<ApiResponse<NewsDto>> {
    return this.http.get<ApiResponse<NewsDto>>(`${this.apiUrl}/${id}`);
  }

  // Get published news
  getPublished(): Observable<ApiResponse<NewsDto[]>> {
    return this.http.get<ApiResponse<NewsDto[]>>(`${this.apiUrl}/published`);
  }

  // Get news by author
  getByAuthor(authorId: number): Observable<ApiResponse<NewsDto[]>> {
    return this.http.get<ApiResponse<NewsDto[]>>(`${this.apiUrl}/author/${authorId}`);
  }

  // Create news
  create(news: CreateNewsDto): Observable<ApiResponse<NewsDto>> {
    return this.http.post<ApiResponse<NewsDto>>(this.apiUrl, news);
  }

  // Update news
  update(id: number, news: UpdateNewsDto): Observable<ApiResponse<NewsDto>> {
    return this.http.put<ApiResponse<NewsDto>>(`${this.apiUrl}/${id}`, news);
  }

  // Publish news
  publish(id: number): Observable<ApiResponse<boolean>> {
    return this.http.patch<ApiResponse<boolean>>(`${this.apiUrl}/${id}/publish`, {});
  }

  // Unpublish news
  unpublish(id: number): Observable<ApiResponse<boolean>> {
    return this.http.patch<ApiResponse<boolean>>(`${this.apiUrl}/${id}/unpublish`, {});
  }

  // Delete news
  delete(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.apiUrl}/${id}`);
  }
}
