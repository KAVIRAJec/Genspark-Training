import { Component } from '@angular/core';
import { Login } from '../login/login';

@Component({
  selector: 'app-profile',
  imports: [],
  templateUrl: './profile.html',
  styleUrl: './profile.css'
})
export class Profile {
  user: Login | null = null;
  constructor() {
    // const userData = localStorage.getItem('user');
    const userData = sessionStorage.getItem('user');
    if (userData) {
      this.user = JSON.parse(userData);
    } else {
      this.user = null;
    }
  }
}
