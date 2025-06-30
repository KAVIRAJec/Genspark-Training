import { TestBed } from '@angular/core/testing';
import { AuthGuard } from './authguard-guard';
import { Router } from '@angular/router';

describe('AuthGuard', () => {
  let guard: AuthGuard;
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(() => {
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    TestBed.configureTestingModule({
      providers: [
        AuthGuard,
        { provide: Router, useValue: routerSpy }
      ]
    });
    guard = TestBed.inject(AuthGuard);
    sessionStorage.clear();
  });

  it('should allow activation if authenticated', () => {
    spyOn(sessionStorage, 'getItem').and.returnValue('token');
    const result = guard.canActivate({} as any, {} as any);
    expect(result).toBeTrue();
    expect(routerSpy.navigate).not.toHaveBeenCalled();
  });

  it('should block activation and redirect if not authenticated', () => {
    spyOn(sessionStorage, 'getItem').and.returnValue(null);
    const result = guard.canActivate({} as any, {} as any);
    expect(result).toBeFalse();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['auth']);
  });
});
