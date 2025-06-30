import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private storageKey = 'notifications';

  getNotifications(): any[] {
    const stored = localStorage.getItem(this.storageKey);
    return stored ? JSON.parse(stored) : [];
  }

  addNotification(notification: any) {
    const notifications = this.getNotifications();
    notifications.unshift(notification);
    localStorage.setItem(this.storageKey, JSON.stringify(notifications));
  }

  markAllAsRead() {
    const notifications = this.getNotifications().map(n => ({ ...n, read: true }));
    localStorage.setItem(this.storageKey, JSON.stringify(notifications));
  }

  clear() {
    localStorage.removeItem(this.storageKey);
  }
}
