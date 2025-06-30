import { TestBed } from '@angular/core/testing';
import { App } from './app';
import { AuthenticationService } from './Services/auth.service';
import { SignalRService } from './Services/signalR.service';
import { of, throwError } from 'rxjs';
import { ActivatedRoute } from '@angular/router';

describe('App', () => {
  let authServiceMock: jasmine.SpyObj<AuthenticationService>;
  let signalRServiceMock: jasmine.SpyObj<SignalRService>;

  beforeEach(async () => {
    authServiceMock = jasmine.createSpyObj('AuthenticationService', ['getMe', 'logout']);
    signalRServiceMock = jasmine.createSpyObj('SignalRService', ['startConnection']);
    await TestBed.configureTestingModule({
      imports: [App],
      providers: [
        { provide: AuthenticationService, useValue: authServiceMock },
        { provide: SignalRService, useValue: signalRServiceMock },
        { provide: ActivatedRoute, useValue: { snapshot: {}, params: {}, queryParams: { subscribe: () => {} } } }
      ]
    }).compileComponents();
    sessionStorage.clear();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it('should call getMe and startConnection if accessToken exists and user is client', () => {
    spyOn(sessionStorage, 'getItem').and.returnValue('token');
    const clientMock = { id: '1', companyName: 'Test', username: 'client', email: '', profileUrl: '', location: '', isActive: true, createdAt: null, updatedAt: null, deletedAt: null, projects: [] };
    authServiceMock.getMe.and.returnValue(of({ data: clientMock, success: true, message: '' }));
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;
    app.tryAutoLogin();
    expect(authServiceMock.getMe).toHaveBeenCalled();
    expect(signalRServiceMock.startConnection).toHaveBeenCalledWith('token', 'client');
  });

  it('should call getMe and startConnection if accessToken exists and user is freelancer', () => {
    spyOn(sessionStorage, 'getItem').and.returnValue('token');
    const freelancerMock = { id: '2', hourlyRate: 10, username: 'freelancer', email: '', profileUrl: '', experienceYears: 2, location: '', isActive: true, createdAt: null, updatedAt: null, deletedAt: null, skills: [] };
    authServiceMock.getMe.and.returnValue(of({ data: freelancerMock, success: true, message: '' }));
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;
    app.tryAutoLogin();
    expect(authServiceMock.getMe).toHaveBeenCalled();
    expect(signalRServiceMock.startConnection).toHaveBeenCalledWith('token', 'freelancer');
  });

  it('should call logout if getMe errors', () => {
    spyOn(sessionStorage, 'getItem').and.returnValue('token');
    authServiceMock.getMe.and.returnValue(throwError(() => new Error('fail')));
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;
    spyOn(console, 'error');
    app.tryAutoLogin();
    expect(authServiceMock.getMe).toHaveBeenCalled();
    expect(authServiceMock.logout).toHaveBeenCalled();
  });

  it('should not call getMe if no accessToken', () => {
    spyOn(sessionStorage, 'getItem').and.returnValue(null);
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;
    app.tryAutoLogin();
    expect(authServiceMock.getMe).not.toHaveBeenCalled();
    expect(signalRServiceMock.startConnection).not.toHaveBeenCalled();
  });
});