import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../environments/environment';
import { Observable, BehaviorSubject } from 'rxjs';
import { NotificationService } from '../Components/notification/notification.service';

@Injectable({ providedIn: 'root' })
export class SignalRService {
  private userRole: 'client' | 'freelancer' | null = null;
  constructor(private notificationService: NotificationService) {}

  private hubConnection!: signalR.HubConnection;

  private chatNotificationSubject = new BehaviorSubject<any>(null);
  chatNotification$: Observable<any> = this.chatNotificationSubject.asObservable();

  private clientNotificationSubject = new BehaviorSubject<any>(null);
  clientNotification$ = this.clientNotificationSubject.asObservable();

  private freelancerNotificationSubject = new BehaviorSubject<any>(null);
  freelancerNotification$ = this.freelancerNotificationSubject.asObservable();

  startConnection(token: string, userRole: 'client' | 'freelancer') {
    if (this.hubConnection) return;
    this.userRole = userRole;

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(
        `${environment.baseUrl.replace('/api/v1', '')}/notificationhub`,
        { accessTokenFactory: () => token }
      )
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => {
        console.log('SignalR Connected!');
        this.hubConnection.onreconnecting((error) => {
          console.warn('SignalR reconnecting...', error);
        });
        this.hubConnection.onreconnected((connectionId) => {
          console.log('SignalR reconnected. Connection ID:', connectionId);
        });
        this.hubConnection.onclose((error) => {
          console.error('SignalR connection closed.', error);
        });
      })
      .catch(err => console.error('SignalR Connection Error:', err));

    // Listen for chat notifications
    this.hubConnection.on('ChatNotification', (message) => {
      this.chatNotificationSubject.next(message);
    });

    // Only listen for relevant notifications
    if (this.userRole === 'client') {
      this.hubConnection.on('ClientNotification', (message) => {
        console.log('Client Notification:', message);
        this.clientNotificationSubject.next(message);
        this.notificationService.addNotification({
          title: message.title || 'Project Notification',
          body: message.body || message.text || JSON.stringify(message),
          time: new Date(),
          read: false
        });
      });
    } else if (this.userRole === 'freelancer') {
      this.hubConnection.on('FreelancerNotification', (notification) => {
        console.log('Freelancer Notification:', notification);
        this.freelancerNotificationSubject.next(notification);
        this.notificationService.addNotification({
          title: notification.title || 'Proposal Notification',
          body: notification.body || notification.text || JSON.stringify(notification),
          time: new Date(),
          read: false
        });
      });
    }
  }

  stopConnection() {
    if (this.hubConnection) {
      this.hubConnection.stop();
      this.hubConnection = undefined!;
    }
  }
}