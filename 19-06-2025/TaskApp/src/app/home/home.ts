import { Component, ElementRef, OnInit, signal, ViewChild } from '@angular/core';
import { User } from '../Models/User.Model';
import { UserService } from '../Services/User.service';
import { FormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged, fromEvent, map } from 'rxjs';

@Component({
  selector: 'app-home',
  imports: [FormsModule],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home implements OnInit {
  users = signal<User[]>([]);
  @ViewChild('searchInput') searchInput!: ElementRef;
  filteredUsers = signal<User[]>([]);

  constructor(private userService: UserService) {
    this.userService.users$.subscribe(users => {
      this.users.set(users);
      this.filteredUsers.set(users);
    });
  }

  ngOnInit(): void {
    this.userService.getUsers();
  }

  ngAfterViewInit(): void {
    fromEvent(this.searchInput.nativeElement, 'input').pipe(
      map((event: any) => event.target.value.toLowerCase()),
      debounceTime(300),
      distinctUntilChanged(),
      map((searchTerm: string) => {
        if (!searchTerm) {
          this.filteredUsers.set(this.users());
        } else {
          const filtered = this.users().filter(user =>
            user.Username.toLowerCase().includes(searchTerm) ||
            user.Email.toLowerCase().includes(searchTerm)
          );
          this.filteredUsers.set(filtered);
        }
      })
    ).subscribe();
  }
}
