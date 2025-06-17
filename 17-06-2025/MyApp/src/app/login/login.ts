import { Component } from '@angular/core';
import { UserService } from '../services/User.service';
import { UserLoginModel } from '../Models/UserLoginModel';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  imports: [FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {
 user:UserLoginModel = new UserLoginModel();
  constructor(private userService: UserService, private router:Router) {}
  handleLogin() {
    this.userService.validateUserLogin(this.user);
    this.router.navigate(['/home/'+ this.user.username]);
  }
}
