import { Component, signal } from '@angular/core';
import { User } from '../Models/User.Model';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { NgFor, NgIf } from '@angular/common';
import { UserService } from '../Services/User.service';

@Component({
  selector: 'app-user-manage',
  imports: [ReactiveFormsModule, NgIf, NgFor],
  templateUrl: './user-manage.html',
  styleUrl: './user-manage.css'
})
export class UserManage {
  userForm: FormGroup;
  isAdded = signal(false);
  isError = signal(false);
  roles = ['Admin', 'User', 'Guest'];

  constructor(private form: FormBuilder, private userService: UserService) {
    this.userForm = this.form.group({
      username: [null, [Validators.required, this.userNameValidator.bind(this)]],
      email: [null, [Validators.required, Validators.email]],
      password: [null, [Validators.required, Validators.minLength(6), this.passwordStrengthValidator]],
      confirmPassword: [null, Validators.required],
      role: [null, Validators.required]
    }, { validators: this.passwordsMatchValidator });
  }

  bannedUsernames: string[] = ['admin', 'user', 'test'];

  userNameValidator(control: AbstractControl): ValidationErrors | null {
    const value: string = control.value || '';
    if (!value) return null;
    const isBanned = this.bannedUsernames.includes(value.toLowerCase());
    return isBanned ? { userName: 'This username is not allowed.' } : null;
  }
  conformPasswordValidator(control: AbstractControl): ValidationErrors | null {
    const value: string = control.value || '';
    if (!value) return null;
    const password = this.userForm.get('password')?.value;
    return password === value ? null : { mustMatch: true };
  }
  passwordStrengthValidator(control: AbstractControl): ValidationErrors | null {
    const value: string = control.value || '';
    if (!value) return null;
    const upperCase = /[A-Z]/.test(value);
    const lowerCase = /[a-z]/.test(value);
    const number = /\d/.test(value);
    const valid = upperCase && lowerCase && number;
    return !valid ? { passwordStrength: 'Password must contain uppercase, lowercase, and a number.' } : null;
  }

  passwordsMatchValidator(group: AbstractControl): ValidationErrors | null {
    const password = group.get('password')?.value;
    const confirmPassword = group.get('confirmPassword')?.value;
    return password === confirmPassword ? null : { mustMatch: true };
  }

  HandleFormSubmit() {
    if (this.userForm.invalid) {
      console.error('Form Validation Error: ', this.userForm.value);
      this.isError.set(true);
      this.isAdded.set(false);
      return;
    }
    const user: User = new User().ToUserModel(this.userForm.value);
    this.userService.addUser(user);
    this.isAdded.set(true);
    this.isError.set(false);
    console.log('User Created: ', user);
  }
}
