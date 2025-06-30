import { TestBed } from '@angular/core/testing';
import { ChatNotificationService } from './chat-notification.service';

describe('ChatNotificationService', () => {
  let service: ChatNotificationService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ChatNotificationService]
    });
    service = TestBed.inject(ChatNotificationService);
    localStorage.clear();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should add and get unread messages', () => {
    service.addNotification({ id: 1, chatRoomId: 'room1' } as any);
    const unread = service.getNotifications().filter(n => n.chatRoomId === 'room1');
    expect(unread.length).toBe(1);
    // expect(unread[0].id).toBe(1);
  });

  it('should clear unread messages for a room', () => {
    service.addNotification({ id: 1, chatRoomId: 'room1' } as any);
    service.clearNotificationsForRoom('room1');
    const unread = service.getNotifications().filter(n => n.chatRoomId === 'room1');
    expect(unread.length).toBe(0);
  });

  it('should get unread count for a room', () => {
    service.addNotification({ id: 1, chatRoomId: 'room1' } as any);
    service.addNotification({ id: 2, chatRoomId: 'room1' } as any);
    expect(service.getUnreadCountForRoom('room1')).toBe(2);
  });
});
