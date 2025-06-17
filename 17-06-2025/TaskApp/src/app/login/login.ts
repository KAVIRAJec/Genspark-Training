import { Component } from '@angular/core';
import { UserService } from '../services/user.service';
import { FormsModule } from '@angular/forms';
import { UserLoginModel } from '../Models/user';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  imports: [FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {
user: UserLoginModel = new UserLoginModel();
loginMessage: string = "";
loginSuccess: boolean = false;

  constructor(private userService: UserService, private router: Router) {}

  handleLogin() {
    this.loginMessage = "";
    this.loginSuccess = false;
    this.userService.LoginUser(this.user);

    this.userService.username$.subscribe({
      next: (username) => {
        if (username) {
          this.loginSuccess = true;
          this.loginMessage = "Login successful!";
          setTimeout(() => {
            this.router.navigate(['products']);
          }, 2000);
        } else {
          this.loginMessage = "Login failed. Please check your credentials.";
        }
      },
      error: (err) => { 
        this.loginMessage = "An error occurred during login. Please try again.";
        console.error(err);
      }
    });
  }
}
