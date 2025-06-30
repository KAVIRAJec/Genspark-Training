import { NgIf, NgClass, NgOptimizedImage } from '@angular/common';
import { Component, OnInit, signal, OnDestroy } from '@angular/core';
import { AuthenticationService } from '../../Services/auth.service';
import { RouterLink, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { ClientModel } from '../../Models/Client.model';
import { FreelancerModel } from '../../Models/Freelancer.model';
import { NotificationComponent } from '../notification/notification';
import { NotificationService } from '../notification/notification.service';

@Component({
  selector: 'app-menubar',
  imports: [NgClass, RouterLink, NgOptimizedImage, NgIf, NotificationComponent],
  templateUrl: './menubar.html',
  styleUrl: './menubar.css'
})
export class Menubar implements OnInit, OnDestroy {
  showMobileMenu = false;
  authenticated = signal<boolean>(false);
  authRole = signal<'freelancer' | 'client' | null>(null);
  showNotificationDropdown = false;
  unreadCount = 0;
  private userSub: Subscription | undefined;
  private notificationInterval: any;

  constructor(
    private authService: AuthenticationService,
    private router: Router,
    private notificationService: NotificationService
  ) { }

  ngOnInit(): void {
    this.userSub = this.authService.user$.subscribe((user: ClientModel | FreelancerModel | null) => {
      if (user && 'companyName' in user) {
        this.authRole.set('client');
        this.authenticated.set(true);
      } else if (user && 'hourlyRate' in user) {
        this.authRole.set('freelancer');
        this.authenticated.set(true);
      } else {
        this.authRole.set(null);
        this.authenticated.set(false);
      }
    });
    this.updateUnreadCount();
    this.notificationInterval = setInterval(() => this.updateUnreadCount(), 2000);
  }

  ngOnDestroy(): void {
    this.userSub?.unsubscribe();
    if (this.notificationInterval) clearInterval(this.notificationInterval);
  }

  toggleMobileMenu() {
    this.showMobileMenu = !this.showMobileMenu;
  }

  navigateWithSearch(target: 'findexpert' | 'findwork', search: string) {
    this.router.navigate([`/${target}`], { queryParams: { search } });
    this.showMobileMenu = false;
  }

  toggleNotificationDropdown() {
    this.showNotificationDropdown = !this.showNotificationDropdown;
  }

  updateUnreadCount() {
    const notifications = this.notificationService.getNotifications();
    this.unreadCount = notifications.filter(n => !n.read).length;
  }
}