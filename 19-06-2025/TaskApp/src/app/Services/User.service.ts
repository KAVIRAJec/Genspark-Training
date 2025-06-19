import { Injectable } from "@angular/core";
import { User } from "../Models/User.Model";
import { BehaviorSubject, tap, take } from 'rxjs';
import { HttpClient } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class UserService {
    private usersSubject = new BehaviorSubject<User[]>([]);
    users$ = this.usersSubject.asObservable();
    apiUrl: string = 'https://dummyjson.com/users';

    constructor(private http: HttpClient) {}

    getUsers() {
        this.users$.pipe(take(1)).subscribe(users => {
            if (!users || users.length === 0) {
                this.http.get<{ users: any[] }>(this.apiUrl).pipe(
                    tap(response => {
                        const mappedUsers = response.users.map(u => new User().ToUserModel(u));
                        this.usersSubject.next([...mappedUsers]);
                    })
                ).subscribe();
            }
        });
    }

    addUser(user: User) {
        this.users$.pipe(
            take(1),
            tap(users => {
                const updatedUsers = [...users, user];
                this.usersSubject.next(updatedUsers);
            })
        ).subscribe();
    }
}