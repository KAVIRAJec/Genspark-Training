import { Component } from '@angular/core';
import { UserModel } from '../Models/User';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { UserService } from '../Services/User.service';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-users',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './users.html',
  styleUrl: './users.css'
})
export class Users {
user: UserModel | undefined;

createUserForm: FormGroup;

constructor(private userService: UserService) {
  this.createUserForm = new FormGroup({
    firstName: new FormControl('', [Validators.required]),
    lastName: new FormControl('', [Validators.required]),
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required]),
    phone: new FormControl('', [Validators.required]),
    age: new FormControl('', [Validators.required, Validators.min(0)]),
    address: new FormControl('', [Validators.required]),
    city: new FormControl('', [Validators.required]),
    state: new FormControl('', [Validators.required]),
    postalCode: new FormControl('', [Validators.required]),
    country: new FormControl('', [Validators.required]),
    role: new FormControl('', [Validators.required])
  });
}

handleCreateUser() {
  if (this.createUserForm.valid) {
    this.user = this.createUserForm.value;
    this.userService.createUser(this.user!).subscribe({
      next: (response) => {
        console.log('User created successfully:', response);
      },
      error: (error) => {
        console.error('Error creating user:', error);
      }
    });
  } else {
    console.log('Form is invalid');
  }
}
}
