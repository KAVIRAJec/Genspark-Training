import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ContactUsDto, CreateContactUsDto, UpdateContactUsDto } from '../../../../Migration-FE/src/app/models/contact-us.model';
import { ApiResponse } from '../models/api-response.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ContactUsService {
  private apiUrl = environment.apiUrl + '/contactus';

  constructor(private http: HttpClient) {}

  sendMessage(message: CreateContactUsDto): Observable<ApiResponse<ContactUsDto>> {
    return this.http.post<ApiResponse<ContactUsDto>>(this.apiUrl, message);
  }

  // Get all contact-us messages
  getAll(): Observable<ApiResponse<ContactUsDto[]>> {
    return this.http.get<ApiResponse<ContactUsDto[]>>(this.apiUrl);
  }

  // Get all contact-us messages with pagination
  getAllPaginated(params: any): Observable<ApiResponse<ContactUsDto[]>> {
    return this.http.get<ApiResponse<ContactUsDto[]>>(`${this.apiUrl}/paginated`, { params });
  }

  // Get contact-us by ID
  getById(id: number): Observable<ApiResponse<ContactUsDto>> {
    return this.http.get<ApiResponse<ContactUsDto>>(`${this.apiUrl}/${id}`);
  }

  // Get unread contact-us messages
  getUnread(): Observable<ApiResponse<ContactUsDto[]>> {
    return this.http.get<ApiResponse<ContactUsDto[]>>(`${this.apiUrl}/unread`);
  }

  // Get contact-us messages by user id
  getByUserId(id: number): Observable<ApiResponse<ContactUsDto[]>> {
    return this.http.get<ApiResponse<ContactUsDto[]>>(`${this.apiUrl}/user/${id}`);
  }

  // Update contact-us
  update(id: number, contactUs: UpdateContactUsDto): Observable<ApiResponse<ContactUsDto>> {
    console.log('Updating contact-us with ID:', id, 'Data:', contactUs);
    return this.http.put<ApiResponse<ContactUsDto>>(`${this.apiUrl}/${id}`, contactUs);
  }

  // Mark as read
  markAsRead(id: number): Observable<boolean> {
    return this.http.put<boolean>(`${this.apiUrl}/mark-as-read/${id}`, {});
  }

  // Delete contact-us
  delete(id: number): Observable<boolean> {
    return this.http.delete<boolean>(`${this.apiUrl}/${id}`);
  }
}
