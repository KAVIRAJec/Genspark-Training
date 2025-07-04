import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, catchError, throwError } from 'rxjs';

// Simplified interfaces for backend responses
export interface BackendOrder {
  id: string;
  amount: number;
  currency: string;
  receipt: string;
  status: string;
}

export interface BackendResponse {
  success: boolean;
  message?: string;
  error?: string;
}

export interface CreateOrderResponse extends BackendResponse {
  order?: BackendOrder;
}

export interface KeyResponse extends BackendResponse {
  key?: string;
}

export interface VerifyResponse extends BackendResponse {
  payment?: {
    order_id: string;
    payment_id: string;
    verified: boolean;
    timestamp: string;
  };
}

@Injectable({
  providedIn: 'root'
})
export class BackendService {
  private readonly baseUrl = 'http://localhost:3000/api';

  constructor(private http: HttpClient) { }

  // Check if backend is available
  isBackendAvailable(): Observable<boolean> {
    return new Observable(observer => {
      this.http.get(`${this.baseUrl}/health`).subscribe({
        next: () => {
          observer.next(true);
          observer.complete();
        },
        error: () => {
          observer.next(false);
          observer.complete();
        }
      });
    });
  }

  // Get Razorpay key from backend
  getPaymentKey(): Observable<KeyResponse> {
    return this.http.get<KeyResponse>(`${this.baseUrl}/payment/key`)
      .pipe(catchError(this.handleError));
  }

  // Create order via backend
  createOrder(amount: number, currency: string = 'INR'): Observable<CreateOrderResponse> {
    const payload = {
      amount,
      currency,
      receipt: `receipt_${Date.now()}`
    };

    return this.http.post<CreateOrderResponse>(`${this.baseUrl}/payment/create-order`, payload)
      .pipe(catchError(this.handleError));
  }

  // Verify payment via backend
  verifyPayment(orderInfo: {
    razorpay_order_id: string;
    razorpay_payment_id: string;
    razorpay_signature: string;
  }): Observable<VerifyResponse> {
    return this.http.post<VerifyResponse>(`${this.baseUrl}/payment/verify`, orderInfo)
      .pipe(catchError(this.handleError));
  }

  // Get payment details
  getPaymentDetails(paymentId: string): Observable<any> {
    return this.http.get(`${this.baseUrl}/payment/${paymentId}`)
      .pipe(catchError(this.handleError));
  }

  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'An unknown error occurred';
    
    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Error: ${error.error.message}`;
    } else {
      // Server-side error
      errorMessage = error.error?.message || `Error Code: ${error.status}\nMessage: ${error.message}`;
    }
    
    console.error('Backend API Error:', errorMessage);
    return throwError(() => new Error(errorMessage));
  }
}
