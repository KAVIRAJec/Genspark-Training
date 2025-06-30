import { TestBed } from '@angular/core/testing';
import { AuthenticationService } from '../auth.service';
import { HttpClient } from '@angular/common/http';
import { of } from 'rxjs';

describe('AuthenticationService', () => {
  let service: AuthenticationService;
  let httpSpy: jasmine.SpyObj<HttpClient>;

  beforeEach(() => {
    httpSpy = jasmine.createSpyObj('HttpClient', ['post', 'get']);
    TestBed.configureTestingModule({
      providers: [
        AuthenticationService,
        { provide: HttpClient, useValue: httpSpy }
      ]
    });
    service = TestBed.inject(AuthenticationService);
    sessionStorage.clear();
  });

  it('should call login and set token on success', () => {
    const mockResponse = { success: true, data: { token: 'abc' } };
    httpSpy.post.and.returnValue(of(mockResponse));
    spyOn(sessionStorage, 'setItem');
    spyOn(service, 'getMe').and.returnValue(of({ success: true, message: '', data: {} as any }));
    service.login('test@mail.com', 'pass').subscribe();
    expect(httpSpy.post).toHaveBeenCalled();
    expect(sessionStorage.setItem).toHaveBeenCalledWith('accessToken', 'abc');
    expect(service.getMe).toHaveBeenCalled();
  });

  it('should call logout and clear token', () => {
    httpSpy.post.and.returnValue(of({}));
    spyOn(sessionStorage, 'removeItem');
    service.logout().subscribe();
    expect(httpSpy.post).toHaveBeenCalled();
    expect(sessionStorage.removeItem).toHaveBeenCalledWith('accessToken');
  });

  it('should call refreshToken and set new token', () => {
    const mockResponse = { success: true, data: { token: 'newtoken' } };
    httpSpy.post.and.returnValue(of(mockResponse));
    spyOn(sessionStorage, 'setItem');
    spyOn(service, 'getMe').and.returnValue(of({ success: true, message: '', data: {} as any }));
    service.refreshToken().subscribe();
    expect(httpSpy.post).toHaveBeenCalled();
    expect(sessionStorage.setItem).toHaveBeenCalledWith('accessToken', 'newtoken');
    expect(service.getMe).toHaveBeenCalled();
  });

  it('should call getMe and update authDetails on success', () => {
    const mockResponse = { success: true, data: { id: '1' } };
    httpSpy.get.and.returnValue(of(mockResponse));
    const nextSpy = spyOn((service as any).authDetails, 'next');
    service.getMe().subscribe();
    expect(httpSpy.get).toHaveBeenCalled();
    expect(nextSpy).toHaveBeenCalledWith({ id: '1' });
  });
});
