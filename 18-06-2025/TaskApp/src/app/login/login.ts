import { Component, OnInit } from '@angular/core';
import { UserLoginModel } from '../Models/User';
import { UserService } from '../Services/User.service';
import { FormControl, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login implements OnInit {
user: UserLoginModel | undefined;
isLoggedIn: boolean = false;

loginForm: FormGroup;

constructor(private userService: UserService, private router: Router) { 
  this.loginForm = new FormGroup({
    username: new FormControl('', [Validators.required]),
    password: new FormControl('', [Validators.required])
  });
}

ngOnInit() {
  const token = localStorage.getItem('accessToken');
  this.isLoggedIn = !!token;
}

handleSubmit() {
  if (this.loginForm.valid) {
    this.userService.loginUser(this.loginForm.value);
    if (this.userService.currentUser$) {
      this.userService.currentUser$.subscribe({
        next: (user) => {
          if (user) {
            console.log('Login successful:', user);
            this.isLoggedIn = true;
            this.router.navigate(['/home']);
          } else {
            console.log('Login failed');
            this.isLoggedIn = false;
          }
        },
        error: (error) => {
          console.error('Error during login:', error);
        }
      });
    }
  }
}

logout() {
  this.userService.logOutUser();
  this.isLoggedIn = false;
}
}
