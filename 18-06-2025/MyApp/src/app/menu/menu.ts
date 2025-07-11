import { Component } from '@angular/core';
import { UserService } from '../services/User.service';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-menu',
  imports: [RouterLink],
  templateUrl: './menu.html',
  styleUrl: './menu.css'
})
export class Menu {
 username$:any;
  username:string|null = "";

  constructor(private userService:UserService)
  {
    //this.username$ = this.userService.username$;
    this.userService.username$.subscribe(
      {
       next:(value) =>{
          this.username = value ;
        },
        error:(err)=>{
          alert(err);
        }
      }
    )
  }
}
