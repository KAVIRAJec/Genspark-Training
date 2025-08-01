import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { OrderDto, CreateOrderDto, UpdateOrderDto } from '../../../../Migration-FE/src/app/models/order.model';
import { ApiResponse } from '../models/api-response.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class OrderService {
  private apiUrl = environment.apiUrl + '/orders';

  constructor(private http: HttpClient) {}

  getAll(): Observable<ApiResponse<OrderDto[]>> {
    return this.http.get<ApiResponse<OrderDto[]>>(this.apiUrl);
  }

  // Get all orders with pagination
  getAllPaginated(params: any): Observable<ApiResponse<OrderDto[]>> {
    return this.http.get<ApiResponse<OrderDto[]>>(`${this.apiUrl}/paginated`, { params });
  }

  // Get order by ID
  getById(id: number): Observable<ApiResponse<OrderDto>> {
    return this.http.get<ApiResponse<OrderDto>>(`${this.apiUrl}/${id}`);
  }

  // Get orders by user ID
  getByUserId(userId: number): Observable<ApiResponse<OrderDto[]>> {
    return this.http.get<ApiResponse<OrderDto[]>>(`${this.apiUrl}/user/${userId}`);
  }

  // Get orders by status
  getByStatus(status: string): Observable<ApiResponse<OrderDto[]>> {
    return this.http.get<ApiResponse<OrderDto[]>>(`${this.apiUrl}/status/${status}`);
  }

  // Create order
  create(order: CreateOrderDto): Observable<ApiResponse<OrderDto>> {
    return this.http.post<ApiResponse<OrderDto>>(this.apiUrl, order);
  }

  // Update order
  update(id: number, order: UpdateOrderDto): Observable<OrderDto> {
    return this.http.put<OrderDto>(`${this.apiUrl}/${id}`, order);
  }

  // Update order status
  updateStatus(id: number, statusDto: any): Observable<OrderDto> {
    return this.http.put<OrderDto>(`${this.apiUrl}/status/${id}`, statusDto);
  }

  // Delete order
  delete(id: number): Observable<boolean> {
    return this.http.delete<boolean>(`${this.apiUrl}/${id}`);
  }
}
