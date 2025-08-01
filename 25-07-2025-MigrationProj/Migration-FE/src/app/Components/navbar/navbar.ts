import { Component, signal } from '@angular/core';
import { RouterLink, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-navbar',
  imports: [RouterLink, CommonModule],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css'
})
export class Navbar {
  menuOpen = false;
  isLoggedIn = signal(false);
  userRole = signal('');

  constructor(private authService: AuthService, private router: Router) {
    this.isLoggedIn.set(this.authService.isLoggedIn());
    this.authService.getCurrentUser().subscribe({
      next: (res) => {
        if (res && res.data) {
          this.userRole.set(res.data?.role);
        } else {
          this.userRole.set('');
        }
      }
    })
  }

  toggleMenu() {
    this.menuOpen = !this.menuOpen;
  }

  handleLogout() {
    this.authService.logout().subscribe({
      next: () => {
        this.isLoggedIn.set(false);
        this.router.navigate(['/login']);
      },
      error: () => {
        this.isLoggedIn.set(false);
        this.router.navigate(['/login']);
      }
    });
  }

  handleLogin() {
    this.router.navigate(['/login']);
  }

  ngDoCheck() {
    this.isLoggedIn.set(this.authService.isLoggedIn());
  }
}
