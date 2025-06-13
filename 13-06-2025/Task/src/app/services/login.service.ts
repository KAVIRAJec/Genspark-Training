import { Injectable } from "@angular/core";
import { LoginModel } from "../Models/Login";

@Injectable()
export class LoginService {
    private users: LoginModel[] = [
        new LoginModel('user1@gmail.com', 'password1'),
        new LoginModel('user2@gmail.com', 'password2')
    ];

    login(user: LoginModel): boolean {
        const foundUser = this.users.find(u => u.email === user.email && u.password === user.password);
        return !!foundUser;
    }
}