import { BehaviorSubject, Observable } from "rxjs";
import { UserLoginModel } from "../Models/UserLoginModel";
import { inject, Injectable } from "@angular/core";
import { HttpClient, HttpHeaders } from "@angular/common/http";

@Injectable()
export class UserService
{
    private http = inject(HttpClient);
    private usernameSubject = new BehaviorSubject<string|null>(null);
    username$:Observable<string|null> = this.usernameSubject.asObservable();

    validateUserLogin(user:UserLoginModel)
    {
        if(user.username.length<3)
        {
            this.usernameSubject.next(null);
        }            
        else
        {
            this.callLoginAPI(user).subscribe(
            {
                next: (data:any) => {
                    this.usernameSubject.next(data.firstName + " " + data.lastName);
                    localStorage.setItem("token",data.accessToken);
                }
            });
        }
    }

    callGetProfile()
    {
        var token = localStorage.getItem("token");
        const httpHeader = new HttpHeaders({
            'Authorization': 'Bearer ' + token
        })
        return this.http.get("https://dummyjson.com/auth/me", { headers: httpHeader });
    }

    callLoginAPI(user:UserLoginModel)
    {
        return this.http.post("https://dummyjson.com/auth/login", user);
    }

    logout(){
        this.usernameSubject.next(null);
    }
}