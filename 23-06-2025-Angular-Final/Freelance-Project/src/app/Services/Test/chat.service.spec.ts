import { TestBed } from '@angular/core/testing';
import { ChatService } from '../chat.service';
import { HttpClient } from '@angular/common/http';
import { of } from 'rxjs';

describe('ChatService', () => {
  let service: ChatService;
  let httpSpy: jasmine.SpyObj<HttpClient>;

  beforeEach(() => {
    httpSpy = jasmine.createSpyObj('HttpClient', ['post', 'put', 'delete', 'get']);
    TestBed.configureTestingModule({
      providers: [
        ChatService,
        { provide: HttpClient, useValue: httpSpy }
      ]
    });
    service = TestBed.inject(ChatService);
  });

  it('should send message', () => {
    httpSpy.post.and.returnValue(of({}));
    service.sendMessage('msg', 'room', 'user').subscribe();
    expect(httpSpy.post).toHaveBeenCalled();
  });

  it('should update message', () => {
    httpSpy.put.and.returnValue(of({}));
    service.updateMessage('id', 'content').subscribe();
    expect(httpSpy.put).toHaveBeenCalled();
  });

  it('should mark message as read', () => {
    httpSpy.put.and.returnValue(of({}));
    service.markMessageAsRead('id', 'room', 'user').subscribe();
    expect(httpSpy.put).toHaveBeenCalled();
  });

  it('should delete message', () => {
    httpSpy.delete.and.returnValue(of({}));
    service.deleteMessage('id', 'room').subscribe();
    expect(httpSpy.delete).toHaveBeenCalled();
  });

  it('should get messages by chat room id', () => {
    httpSpy.get.and.returnValue(of({}));
    service.getMessagesByChatRoomId('room').subscribe();
    expect(httpSpy.get).toHaveBeenCalled();
  });

  it('should get chat room by project id', () => {
    httpSpy.get.and.returnValue(of({}));
    service.getChatRoomByProjectId('project').subscribe();
    expect(httpSpy.get).toHaveBeenCalled();
  });

  it('should get chat rooms by user id', () => {
    httpSpy.get.and.returnValue(of({}));
    service.getChatRoomsByUserId('user').subscribe();
    expect(httpSpy.get).toHaveBeenCalled();
  });
});
