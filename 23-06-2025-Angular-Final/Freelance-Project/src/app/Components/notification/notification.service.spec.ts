import { TestBed } from '@angular/core/testing';
import { NotificationService } from './notification.service';

describe('NotificationService', () => {
  let service: NotificationService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [NotificationService]
    });
    service = TestBed.inject(NotificationService);
    localStorage.clear();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should store and retrieve notifications', () => {
    service.addNotification({ id: 1 } as any);
    const notifs = service.getNotifications();
    expect(notifs.length).toBe(1);
    expect(notifs[0].id).toBe(1);
  });

  it('should clear notifications', () => {
    service.addNotification({ id: 1 } as any);
    service.clear();
    const notifs = service.getNotifications();
    expect(notifs.length).toBe(0);
  });
});
