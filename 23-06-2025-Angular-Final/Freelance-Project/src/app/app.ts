import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Menubar } from "./Components/menubar/menubar";
import { AuthenticationService } from './Services/auth.service';
import { SignalRService } from './Services/signalR.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Menubar],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  constructor(
    private authService: AuthenticationService,
    private signalRService: SignalRService
  ) {}
  protected title = 'Freelancing Project';

  ngOnInit(): void {
    this.tryAutoLogin();
  }

  tryAutoLogin() {
    const accessToken = sessionStorage.getItem('accessToken');
    if (accessToken) {
      console.log('Access token found, trying to get user information.');
      this.authService.getMe().subscribe({
        next: (response) => {
          console.log('User information restored successfully');
          const role = ('companyName' in response.data) ? 'client' : 'freelancer';
          this.signalRService.startConnection(accessToken, role);
        },
        error: (err) => {
          console.error('Failed to fetch user information:', err);
          this.authService.logout();
        }
      });
    }
  }
}