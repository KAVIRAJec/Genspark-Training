import { Component } from '@angular/core';
import { UserService } from '../services/User.service';
import { UserLoginModel } from '../Models/UserLoginModel';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { textValidator } from '../Misc/textValidator';

@Component({
  selector: 'app-login',
  imports: [FormsModule, ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {
  user:UserLoginModel = new UserLoginModel();
  loginForm: FormGroup;

  constructor(private userService: UserService, private router:Router) {
    this.loginForm = new FormGroup({
      un: new FormControl(null, Validators.required),
      pass: new FormControl(null, [Validators.required,textValidator()])
    });
  }

  public get un(): any {
    return this.loginForm.get('un');
  }
  public get pass(): any {
    return this.loginForm.get('pass');
  }

  handleLogin() {
    if (this.loginForm.invalid) return;

    this.userService.validateUserLogin(this.loginForm.value);
    this.router.navigate(['/home/'+ this.loginForm.value.un]);
  }
}
