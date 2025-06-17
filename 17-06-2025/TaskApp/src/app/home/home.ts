import { Component } from '@angular/core';
import { UserService } from '../services/user.service';
import { UserModel } from '../Models/user';

@Component({
  selector: 'app-home',
  imports: [],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home {
userProfile: UserModel = new UserModel();
username$: any;

  constructor(private userService: UserService) { }

  ngOnInit() {
    this.userService.username$.subscribe({
      next: (value) => {
        this.username$ = value;
        const profileObs = this.userService.GetUserProfile();
        if (profileObs) {
          profileObs.subscribe({
            next: (profile) => {
              this.userProfile = profile as UserModel;
            },
            error: (err) => {
              console.error("Error fetching user profile:", err);
            }
          });
        }
      }
    });
  }
}