import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Chat } from './chat';
import { ChatService } from '../../Services/chat.service';
import { AuthenticationService } from '../../Services/auth.service';
import { ToastrService } from 'ngx-toastr';
import { Store } from '@ngrx/store';
import { SignalRService } from '../../Services/signalR.service';
import { ChatNotificationService } from './chat-notification.service';
import { of, Subject } from 'rxjs';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('Chat', () => {
  let component: Chat;
  let fixture: ComponentFixture<Chat>;
  let chatServiceSpy: jasmine.SpyObj<ChatService>;
  let authServiceSpy: jasmine.SpyObj<AuthenticationService>;
  let toastrSpy: jasmine.SpyObj<ToastrService>;
  let storeSpy: jasmine.SpyObj<Store<any>>;
  let signalRServiceSpy: jasmine.SpyObj<SignalRService>;
  let chatNotificationServiceSpy: jasmine.SpyObj<ChatNotificationService>;
  let userSubject: Subject<any>;

  beforeEach(async () => {
    chatServiceSpy = jasmine.createSpyObj('ChatService', ['getChatRoomsByUserId']);
    authServiceSpy = jasmine.createSpyObj('AuthenticationService', [], { user$: new Subject() });
    toastrSpy = jasmine.createSpyObj('ToastrService', ['info', 'error']);
    storeSpy = jasmine.createSpyObj('Store', ['dispatch', 'select']);
    signalRServiceSpy = jasmine.createSpyObj('SignalRService', [], { chatNotification$: of(null) });
    chatNotificationServiceSpy = jasmine.createSpyObj('ChatNotificationService', ['addNotification', 'clearNotificationsForRoom', 'getUnreadCountForRoom', 'getNotifications']);
    userSubject = new Subject();
    Object.defineProperty(authServiceSpy, 'user$', { get: () => userSubject.asObservable() });
    storeSpy.select.and.returnValue(of([]));

    await TestBed.configureTestingModule({
      imports: [Chat],
      providers: [
        { provide: ChatService, useValue: chatServiceSpy },
        { provide: AuthenticationService, useValue: authServiceSpy },
        { provide: ToastrService, useValue: toastrSpy },
        { provide: Store, useValue: storeSpy },
        { provide: SignalRService, useValue: signalRServiceSpy },
        { provide: ChatNotificationService, useValue: chatNotificationServiceSpy }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    fixture = TestBed.createComponent(Chat);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should fetch chat rooms on user emit', () => {
    const user = { id: 'u1' };
    chatServiceSpy.getChatRoomsByUserId.and.returnValue(
      of({
        success: true,
        message: '',
        data: {
          data: [],
          pagination: { page: 1, pageSize: 10, totalRecords: 0, totalPages: 0 }
        }
      })
    );
    userSubject.next(user);
    expect(chatServiceSpy.getChatRoomsByUserId).toHaveBeenCalledWith('u1');
  });

  it('should select a room and clear notifications', () => {
    const room = { id: 'room1' } as any;
    component.selectRoom(room);
    expect(component.selectedRoom()).toBe(room);
    expect(chatNotificationServiceSpy.clearNotificationsForRoom).toHaveBeenCalledWith('room1');
    expect(storeSpy.dispatch).toHaveBeenCalled();
  });
});
