import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NotificationComponent } from './notification';
import { NotificationService } from './notification.service';
import { By } from '@angular/platform-browser';

describe('NotificationComponent', () => {
  let component: NotificationComponent;
  let fixture: ComponentFixture<NotificationComponent>;
  let mockNotificationService: jasmine.SpyObj<NotificationService>;

  beforeEach(async () => {
    mockNotificationService = jasmine.createSpyObj('NotificationService', [
      'getNotifications', 'markAllAsRead', 'clear', 'addNotification'
    ]);
    mockNotificationService.getNotifications.and.returnValue([
      { id: 1, message: 'Test', read: false },
      { id: 2, message: 'Another', read: true }
    ]);

    await TestBed.configureTestingModule({
      imports: [NotificationComponent],
      providers: [
        { provide: NotificationService, useValue: mockNotificationService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(NotificationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load notifications on init', () => {
    expect(component.notifications.length).toBe(2);
    expect(component.notifications[0].message).toBe('Test');
  });

  it('should mark all as read and refresh notifications', () => {
    mockNotificationService.getNotifications.and.returnValue([
      { id: 1, message: 'Test', read: true },
      { id: 2, message: 'Another', read: true }
    ]);
    component.markAllAsRead();
    expect(mockNotificationService.markAllAsRead).toHaveBeenCalled();
    expect(component.notifications.every(n => n.read)).toBeTrue();
  });

  it('should clear notifications', () => {
    component.clearNotifications();
    expect(mockNotificationService.clear).toHaveBeenCalled();
    expect(component.notifications.length).toBe(0);
  });
});
