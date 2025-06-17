import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { BehaviorSubject, map, Observable } from "rxjs";
import { UserLoginModel } from "../Models/user";

@Injectable()
export class UserService {
    http = inject(HttpClient);
    private usernameSubject = new BehaviorSubject<string|null>(null);
    username$:Observable<string|null> = this.usernameSubject.asObservable();

    LoginUser(user: UserLoginModel)
    {
        if (this.validateUserLogin(user)) {
            this.callLoginAPI(user).subscribe({
                next: (data: any) => {
                    this.usernameSubject.next(data.firstName + " " + data.lastName);
                    localStorage.setItem("token", data.accessToken);
                },
                error: (err) => {
                    this.usernameSubject.next(null);
                }
            });
        } else {
            this.usernameSubject.next(null);
        }
    }

    LogoutUser() { 
        this.usernameSubject.next(null);
        localStorage.removeItem("token");
    }
    GetUserProfile(): Observable<any> | null { 
        const token = localStorage.getItem("token");
        if (token) {
            return this.http.get("https://dummyjson.com/auth/me", {
                headers: {
                    Authorization: `Bearer ${token}`
                }
            });
        } return null;
    }

    validateUserLogin(user: UserLoginModel) {
        if (user.username && user.username.length >= 3 && user.password && user.password.length >= 6) {
            return true;
        } else {
            this.usernameSubject.next(null);
            return false;
        }
    }

    callLoginAPI(user: UserLoginModel) {
        return this.http.post("https://dummyjson.com/auth/login", user);
    }
}
