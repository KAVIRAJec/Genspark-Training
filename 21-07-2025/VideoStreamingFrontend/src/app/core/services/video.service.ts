import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { 
  VideoResponseDto, 
  VideoListResponseDto, 
  VideoUploadRequestDto, 
  ApiResponseDto,
  PaginationRequestDto 
} from '../models/video.models';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class VideoService {
  private readonly baseUrl = environment.apiUrl + '/api/videos';
  private videosSubject = new BehaviorSubject<VideoResponseDto[]>([]);
  public videos$ = this.videosSubject.asObservable();

  constructor(private http: HttpClient) {}

  uploadVideo(request: VideoUploadRequestDto): Observable<ApiResponseDto<VideoResponseDto>> {
    const formData = new FormData();
    formData.append('title', request.title);
    if (request.description) {
      formData.append('description', request.description);
    }
    formData.append('videoFile', request.videoFile);

    return this.http.post<ApiResponseDto<VideoResponseDto>>(`${this.baseUrl}/upload`, formData);
  }

  getVideos(pagination: PaginationRequestDto): Observable<ApiResponseDto<VideoListResponseDto>> {
    let params = new HttpParams()
      .set('page', pagination.page.toString())
      .set('pageSize', pagination.pageSize.toString())
      .set('sortDescending', pagination.sortDescending.toString());

    if (pagination.searchTerm) {
      params = params.set('searchTerm', pagination.searchTerm);
    }

    if (pagination.sortBy) {
      params = params.set('sortBy', pagination.sortBy);
    }

    return this.http.get<ApiResponseDto<VideoListResponseDto>>(this.baseUrl, { params });
  }

  getVideoById(id: number): Observable<ApiResponseDto<VideoResponseDto>> {
    return this.http.get<ApiResponseDto<VideoResponseDto>>(`${this.baseUrl}/${id}`);
  }

  deleteVideo(id: number): Observable<ApiResponseDto<boolean>> {
    return this.http.delete<ApiResponseDto<boolean>>(`${this.baseUrl}/${id}`);
  }

  updateVideo(id: number, request: Partial<VideoUploadRequestDto>): Observable<ApiResponseDto<VideoResponseDto>> {
    return this.http.put<ApiResponseDto<VideoResponseDto>>(`${this.baseUrl}/${id}`, request);
  }

  getVideoStreamUrl(id: number): string {
    return `${this.baseUrl}/${id}/stream`;
  }

  // Health check
  healthCheck(): Observable<any> {
    return this.http.get(`${this.baseUrl}/health`);
  }

  // Update local videos cache
  updateVideosCache(videos: VideoResponseDto[]): void {
    this.videosSubject.next(videos);
  }

  // Get cached videos
  getCachedVideos(): VideoResponseDto[] {
    return this.videosSubject.value;
  }
}
