import { Component, OnInit } from '@angular/core';
import { NotificationService } from './notification.service';
import { DatePipe, NgFor, NgIf } from '@angular/common';

@Component({
  selector: 'app-notification',
  imports: [DatePipe, NgIf, NgFor],
  templateUrl: './notification.html',
  styleUrls: ['./notification.css']
})
export class NotificationComponent implements OnInit {
  notifications: any[] = [];

  constructor(private notificationService: NotificationService) {}

  ngOnInit() {
    this.notifications = this.notificationService.getNotifications();
  }

  markAllAsRead() {
    this.notificationService.markAllAsRead();
    this.notifications = this.notificationService.getNotifications();
  }

  clearNotifications() {
    this.notificationService.clear();
    this.notifications = [];
  }
}
