import { TestBed } from '@angular/core/testing';
import { HttpInterceptorFn } from '@angular/common/http';

import { AuthInterceptor } from './auth-interceptor';

describe('AuthInterceptor', () => {
  let nextSpy: jasmine.Spy;
  let reqMock: any;

  beforeEach(() => {
    nextSpy = jasmine.createSpy('next');
    reqMock = { clone: jasmine.createSpy('clone').and.callFake((opts: any) => ({ ...reqMock, ...opts })) };
  });

  afterEach(() => {
    sessionStorage.clear();
  });

  it('should add Authorization header if token exists', () => {
    spyOn(sessionStorage, 'getItem').and.returnValue('mock-token');
    AuthInterceptor(reqMock as any, nextSpy as any);
    expect(reqMock.clone).toHaveBeenCalledWith({ setHeaders: { Authorization: 'Bearer mock-token' } });
    expect(nextSpy).toHaveBeenCalled();
  });

  it('should not add Authorization header if no token', () => {
    spyOn(sessionStorage, 'getItem').and.returnValue(null);
    AuthInterceptor(reqMock as any, nextSpy as any);
    expect(reqMock.clone).not.toHaveBeenCalled();
    expect(nextSpy).toHaveBeenCalledWith(reqMock);
  });
});
