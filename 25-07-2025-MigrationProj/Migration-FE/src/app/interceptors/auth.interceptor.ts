import { inject, Injectable } from '@angular/core';
import {
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';
import { ToastService } from '../Components/Toast/toast.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private isLoggingOut = false;
  constructor(private authService: AuthService, private toast: ToastService) {}
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const token = localStorage.getItem('accessToken');
    let authReq = req;
    if (token) {
      authReq = req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });
    }
    return next.handle(authReq).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401 && !this.isLoggingOut) {
          this.isLoggingOut = true;
          this.authService.logout().subscribe({
            next: () => {
              this.toast.show('Session expired, Please login again.', 'error');
            },
            error: () => {
              this.isLoggingOut = false;
            }
          });
        }
        return throwError(() => error);
      })
    );
  }
}
