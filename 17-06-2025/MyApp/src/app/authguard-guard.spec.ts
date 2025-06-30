import { TestBed } from '@angular/core/testing';
import { CanActivateFn } from '@angular/router';

import { AuthGuard } from './authguard-guard';

import { Router } from '@angular/router';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';

describe('authguardGuard', () => {
  let routerSpy: jasmine.SpyObj<Router>;

  const executeGuard: CanActivateFn = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) =>
    TestBed.runInInjectionContext(() => new AuthGuard(routerSpy).canActivate(route, state));

  beforeEach(() => {
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    TestBed.configureTestingModule({
      providers: [{ provide: Router, useValue: routerSpy }]
    });
  });

  it('should be created', () => {
    expect(executeGuard).toBeTruthy();
  });
});
