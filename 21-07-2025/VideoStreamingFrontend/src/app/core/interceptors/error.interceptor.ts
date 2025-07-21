import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, catchError, throwError } from 'rxjs';
import { NotificationService } from '../services/notification.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(private notificationService: NotificationService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        let errorMessage = 'An unexpected error occurred';

        if (error.error instanceof ErrorEvent) {
          // Client-side error
          errorMessage = error.error.message;
        } else {
          // Server-side error
          if (error.status === 0) {
            errorMessage = 'Unable to connect to the server. Please check your internet connection.';
          } else if (error.status >= 400 && error.status < 500) {
            errorMessage = error.error?.message || `Client error: ${error.status}`;
          } else if (error.status >= 500) {
            errorMessage = 'Server error. Please try again later.';
          }
        }

        this.notificationService.showError(errorMessage);
        return throwError(() => error);
      })
    );
  }
}
