import { TestBed } from '@angular/core/testing';
import { HttpRequest, HttpHandlerFn, HttpErrorResponse, HttpEvent } from '@angular/common/http';
import { refreshTokenInterceptor } from './refresh-token-interceptor';
import { of, throwError } from 'rxjs';
import { AuthenticationService } from '../Services/auth.service';

describe('refreshTokenInterceptor', () => {
  let nextSpy: jasmine.Spy;
  let reqMock: any;
  let mockAuthService: any;

  beforeEach(() => {
    nextSpy = jasmine.createSpy('next');
    reqMock = {
      clone: jasmine.createSpy('clone').and.callFake((opts: any) => ({ ...reqMock, ...opts })),
      headers: { set: jasmine.createSpy('set').and.returnValue({}) }
    };
    mockAuthService = jasmine.createSpyObj('AuthenticationService', ['refreshToken']);
    sessionStorage.clear();
    TestBed.configureTestingModule({
      providers: [
        { provide: AuthenticationService, useValue: mockAuthService }
      ]
    });
  });

  function runInterceptor(req: any, next: any) {
    return TestBed.runInInjectionContext(() => refreshTokenInterceptor(req, next));
  }

  it('should pass through if no error', (done) => {
    nextSpy.and.returnValue(of({} as HttpEvent<unknown>));
    runInterceptor(reqMock as any, nextSpy as any).subscribe((res) => {
      expect(res).toBeDefined();
      done();
    });
  });

  it('should handle 401 error and refresh token', (done) => {
    const error = new HttpErrorResponse({ status: 401, error: 'Unauthorized' });
    const tokenResponse = { success: true, message: '', data: { token: 'new-token' } };
    nextSpy.and.returnValues(throwError(() => error), of({} as HttpEvent<unknown>));
    mockAuthService.refreshToken.and.returnValue(of(tokenResponse));
    spyOn(sessionStorage, 'setItem');
    runInterceptor(reqMock as any, nextSpy as any).subscribe((res) => {
      expect(mockAuthService.refreshToken).toHaveBeenCalled();
      expect(sessionStorage.setItem).toHaveBeenCalledWith('accessToken', 'new-token');
      expect(nextSpy).toHaveBeenCalled();
      expect(res).toBeDefined();
      done();
    });
  });

  it('should handle refresh token failure', (done) => {
    const error = new HttpErrorResponse({ status: 401, error: 'Unauthorized' });
    nextSpy.and.returnValue(throwError(() => error));
    mockAuthService.refreshToken.and.returnValue(throwError(() => new Error('fail')));
    runInterceptor(reqMock as any, nextSpy as any).subscribe({
      error: (err) => {
        expect(mockAuthService.refreshToken).toHaveBeenCalled();
        expect(err.message).toBe('fail');
        done();
      }
    });
  });
});
