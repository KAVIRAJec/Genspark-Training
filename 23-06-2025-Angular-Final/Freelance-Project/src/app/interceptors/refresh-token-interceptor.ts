import { HttpEvent, HttpHandlerFn, HttpInterceptorFn, HttpRequest, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Observable, throwError, BehaviorSubject } from 'rxjs';
import { catchError, switchMap, filter, take, tap } from 'rxjs/operators';
import { AuthenticationService } from '../Services/auth.service';

const isRefreshing = { value: false };
const refreshTokenSubject = new BehaviorSubject<string | null>(null);

export const refreshTokenInterceptor: HttpInterceptorFn = (req: HttpRequest<any>, next: HttpHandlerFn): Observable<HttpEvent<any>> => {
  const authService = inject(AuthenticationService);

  const handle401Error = (request: HttpRequest<any>, nextHandler: HttpHandlerFn): Observable<HttpEvent<any>> => {
    console.log('[REFRESH-INTERCEPTOR] handle401Error called');
    if (!isRefreshing.value) {
      isRefreshing.value = true;
      refreshTokenSubject.next(null);
      // console.log('[REFRESH-INTERCEPTOR] Refreshing token...');
      return authService.refreshToken().pipe(
        switchMap((tokenResponse: any) => {
          isRefreshing.value = false;
          // Expecting ApiResponse<AuthenticationModel>
          const newToken = tokenResponse?.data?.token;
          if (!newToken) {
            // console.error('[REFRESH-INTERCEPTOR] No token in refresh response', tokenResponse);
            return throwError(() => new Error('No token in refresh response'));
          }
          sessionStorage.setItem('accessToken', newToken);
          refreshTokenSubject.next(newToken);
          const cloned = request.clone({
            headers: request.headers.set('Authorization', `Bearer ${newToken}`),
          });
          return nextHandler(cloned);
        }),
        catchError((err) => {
          isRefreshing.value = false;
          // console.error('[REFRESH-INTERCEPTOR] Refresh token failed', err);
          return throwError(() => err);
        })
      );
    } else {
      return refreshTokenSubject.pipe(
        filter(token => token !== null),
        take(1),
        // tap(() => console.log('[REFRESH-INTERCEPTOR] Using existing refresh token')),
        switchMap(token => {
          const cloned = request.clone({
            headers: request.headers.set('Authorization', `Bearer ${token}`),
          });
          return nextHandler(cloned);
        })
      );
    }
  };

  return next(req).pipe(
    catchError(error => {
      // console.log('[REFRESH-INTERCEPTOR] catchError', error);
      if (error instanceof HttpErrorResponse && error.status === 401) {
        return handle401Error(req, next);
      }
      return throwError(() => error);
    })
  );
};
