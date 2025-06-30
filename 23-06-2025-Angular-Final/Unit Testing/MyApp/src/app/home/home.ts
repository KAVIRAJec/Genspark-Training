import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, RouterOutlet } from '@angular/router';
import { UserService } from '../services/User.service';

@Component({
  selector: 'app-home',
  imports: [RouterOutlet],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home implements OnInit {
uname = "";
router = inject(ActivatedRoute);

constructor(private userService: UserService){ }

ngOnInit() {
  console.log("Home Component Initialized");
  this.uname = this.router.snapshot.params['un'];
  if (this.uname == null) {
    this.userService.username$.subscribe({
      next: (data) => {
        this.uname = data as string;
      }
    });
  }
}

}
