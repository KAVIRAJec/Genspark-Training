import { Component, inject } from '@angular/core';
import { UserService } from '../services/User.service';
import { UserModel } from '../Models/UserModel';

@Component({
  selector: 'app-profile',
  imports: [],
  templateUrl: './profile.html',
  styleUrl: './profile.css'
})
export class Profile {
  userService = inject(UserService);
  profileData: UserModel = new UserModel();

  constructor() {
    this.userService.callGetProfile().subscribe({
      next: (data: any) => {
        this.profileData = UserModel.fromForm(data);
      }
    });
  }

}
