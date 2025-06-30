import { TestBed } from '@angular/core/testing';
import { HttpInterceptorFn, HttpRequest, HttpHandlerFn, HttpErrorResponse, HttpEvent } from '@angular/common/http';
import { httpErrorInterceptor } from './http-error-interceptor';
import { of, throwError } from 'rxjs';
import { Router } from '@angular/router';

describe('httpErrorInterceptor', () => {
  let nextSpy: jasmine.Spy;
  let reqMock: any;
  let routerMock: any;

  beforeEach(() => {
    nextSpy = jasmine.createSpy('next');
    reqMock = {};
    routerMock = {};
    spyOn(console, 'error');
    TestBed.configureTestingModule({
      providers: [
        { provide: Router, useValue: routerMock }
      ]
    });
  });

  function runInterceptor(req: any, next: any) {
    return TestBed.runInInjectionContext(() => httpErrorInterceptor(req, next));
  }

  it('should pass through if no error', (done) => {
    nextSpy.and.returnValue(of({} as HttpEvent<unknown>));
    runInterceptor(reqMock as any, nextSpy as any).subscribe((res) => {
      expect(res).toBeDefined();
      done();
    });
  });

  it('should handle HttpErrorResponse with object error', (done) => {
    const error = new HttpErrorResponse({ status: 400, error: { message: 'fail' }, statusText: 'Bad Request' });
    nextSpy.and.returnValue(throwError(() => error));
    runInterceptor(reqMock as any, nextSpy as any).subscribe({
      error: (err) => {
        expect(console.error).toHaveBeenCalled();
        expect(err).toEqual({ message: 'fail' });
        done();
      }
    });
  });

  it('should handle HttpErrorResponse with string error', (done) => {
    const error = new HttpErrorResponse({ status: 500, error: 'fail', statusText: 'Server Error' });
    nextSpy.and.returnValue(throwError(() => error));
    runInterceptor(reqMock as any, nextSpy as any).subscribe({
      error: (err) => {
        expect(console.error).toHaveBeenCalled();
        expect(err).toBe('fail');
        done();
      }
    });
  });
});
