import { Injectable } from '@angular/core';
import { ChatMessageModel } from '../../Models/ChatMessage.model';

@Injectable({ providedIn: 'root' })
export class ChatNotificationService {
  private storageKey = 'chat-notifications';

  getNotifications(): ChatMessageModel[] {
    const stored = localStorage.getItem(this.storageKey);
    return stored ? JSON.parse(stored) : [];
  }

  addNotification(message: ChatMessageModel) {
    const notifications = this.getNotifications();
    notifications.unshift(message);
    localStorage.setItem(this.storageKey, JSON.stringify(notifications));
  }

  clearNotificationsForRoom(chatRoomId: string) {
    const notifications = this.getNotifications().filter(n => n.chatRoomId !== chatRoomId);
    localStorage.setItem(this.storageKey, JSON.stringify(notifications));
  }

  getUnreadCountForRoom(chatRoomId: string): number {
    return this.getNotifications().filter(n => n.chatRoomId === chatRoomId).length;
  }

  clearAll() {
    localStorage.removeItem(this.storageKey);
  }
}
