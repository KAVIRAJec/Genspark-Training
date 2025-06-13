import { Component } from '@angular/core';
import { LoginService } from '../services/login.service';
import { LoginModel } from '../Models/Login';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  imports: [FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {
  email: string = '';
  password: string = '';
  message: string = '';
  constructor(private loginService: LoginService, private router: Router) { }

  onSubmit() {
    if(this.email === '' || this.password === '') {
      this.message = 'Email or password cannot be empty!';
      return;
    }
    if(!this.email.includes('@')) {
      this.message = 'Invalid email format!';
      return;
    }
    if(this.password.length < 5) {
      this.message = 'Password must be at least 5 characters long!';
      return;
    }
    this.login();
  }

  login() {
    const user: LoginModel = new LoginModel(this.email, this.password);
    if (this.loginService.login(user)) {
      // localStorage.setItem('user', JSON.stringify(user));
      sessionStorage.setItem('user', JSON.stringify(user));
      this.message = 'Login successful!';
      // this.router.navigate(['/profile']);
    } else {
      this.message = 'Invalid credentials!';
    }
  }
}
