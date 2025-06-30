import { Component } from '@angular/core';
import { Store } from '@ngrx/store';
import { User } from '../Models/User';
import { addUser } from '../ngrx/users.actions';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-add-user',
  imports: [FormsModule],
  templateUrl: './add-user.html',
  styleUrl: './add-user.css'
})
export class AddUser {
  newUser: User = new User();
  constructor(private store: Store) { }

  handleAddUser() {
    this.store.dispatch(addUser({ user: this.newUser }));
  }
}
