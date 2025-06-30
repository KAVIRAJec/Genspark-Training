import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Menubar } from './menubar';
import { AuthenticationService } from '../../Services/auth.service';
import { ActivatedRoute, Router, UrlTree } from '@angular/router';
import { NotificationService } from '../notification/notification.service';
import { BehaviorSubject, of } from 'rxjs';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('Menubar', () => {
  let fixture: ComponentFixture<Menubar>;
  let component: Menubar;
  let authServiceMock: jasmine.SpyObj<AuthenticationService>;
  let routerMock: jasmine.SpyObj<Router>;
  let notificationServiceMock: jasmine.SpyObj<NotificationService>;
  let userSubject: BehaviorSubject<any>;

  beforeEach(async () => {
    userSubject = new BehaviorSubject<any>(null);
    authServiceMock = jasmine.createSpyObj('AuthenticationService', [], { user$: userSubject.asObservable() });
    // Add createUrlTree, serializeUrl, and events to Router mock
    routerMock = jasmine.createSpyObj('Router', ['navigate', 'createUrlTree', 'serializeUrl']);
    routerMock.createUrlTree.and.returnValue({} as UrlTree);
    routerMock.serializeUrl.and.returnValue('/dummy');
    // Add events observable for RouterLink
    (routerMock as any).events = of();
    notificationServiceMock = jasmine.createSpyObj('NotificationService', ['getNotifications']);
    notificationServiceMock.getNotifications.and.returnValue([
      { read: false }, { read: true }, { read: false }
    ]);

    await TestBed.configureTestingModule({
      imports: [Menubar],
      providers: [
        { provide: AuthenticationService, useValue: authServiceMock },
        { provide: Router, useValue: routerMock },
        { provide: NotificationService, useValue: notificationServiceMock },
        { provide: ActivatedRoute, useValue: { snapshot: {}, params: {}, queryParams: { subscribe: () => {} } } }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();
  });

  afterEach(() => {
    if (component && component.ngOnDestroy) {
      component.ngOnDestroy();
    }
  });

  function setUserAndDetect(user: any) {
    userSubject.next(user);
    fixture.detectChanges();
  }

  it('should create', () => {
    fixture = TestBed.createComponent(Menubar);
    component = fixture.componentInstance;
    expect(component).toBeTruthy();
  });

  it('should set authenticated and authRole for client', () => {
    fixture = TestBed.createComponent(Menubar);
    component = fixture.componentInstance;
    setUserAndDetect({ companyName: 'TestCo', id: '1' });
    expect(component.authenticated()).toBeTrue();
    expect(component.authRole()).toBe('client');
  });

  it('should set authenticated and authRole for freelancer', () => {
    fixture = TestBed.createComponent(Menubar);
    component = fixture.componentInstance;
    setUserAndDetect({ hourlyRate: 10, id: '2' });
    expect(component.authenticated()).toBeTrue();
    expect(component.authRole()).toBe('freelancer');
  });

  it('should set authenticated false and authRole null for no user', () => {
    fixture = TestBed.createComponent(Menubar);
    component = fixture.componentInstance;
    setUserAndDetect(null);
    expect(component.authenticated()).toBeFalse();
    expect(component.authRole()).toBeNull();
  });

  it('should toggle mobile menu', () => {
    fixture = TestBed.createComponent(Menubar);
    component = fixture.componentInstance;
    expect(component.showMobileMenu).toBeFalse();
    component.toggleMobileMenu();
    expect(component.showMobileMenu).toBeTrue();
  });

  it('should navigate with search and close menu', () => {
    fixture = TestBed.createComponent(Menubar);
    component = fixture.componentInstance;
    component.showMobileMenu = true;
    component.navigateWithSearch('findexpert', 'Angular');
    expect(routerMock.navigate).toHaveBeenCalledWith(['/findexpert'], { queryParams: { search: 'Angular' } });
    expect(component.showMobileMenu).toBeFalse();
  });

  it('should toggle notification dropdown', () => {
    fixture = TestBed.createComponent(Menubar);
    component = fixture.componentInstance;
    expect(component.showNotificationDropdown).toBeFalse();
    component.toggleNotificationDropdown();
    expect(component.showNotificationDropdown).toBeTrue();
  });

  it('should update unread count from notificationService', () => {
    fixture = TestBed.createComponent(Menubar);
    component = fixture.componentInstance;
    component.updateUnreadCount();
    expect(notificationServiceMock.getNotifications).toHaveBeenCalled();
    expect(component.unreadCount).toBe(2); // 2 unread
  });

  it('should clear interval and unsubscribe on destroy', () => {
    fixture = TestBed.createComponent(Menubar);
    component = fixture.componentInstance;
    spyOn(window, 'clearInterval');
    const unsubSpy = jasmine.createSpy('unsubscribe');
    (component as any).userSub = { unsubscribe: unsubSpy };
    (component as any).notificationInterval = 123;
    component.ngOnDestroy();
    expect(unsubSpy).toHaveBeenCalled();
    expect(window.clearInterval).toHaveBeenCalledWith(123);
  });
});
