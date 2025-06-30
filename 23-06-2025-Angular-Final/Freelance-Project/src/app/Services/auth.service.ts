import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { AuthenticationModel } from "../Models/Authentication.model";
import { BehaviorSubject, tap } from "rxjs";
import { ApiResponse } from "../Models/ApiResponse.model";
import { FreelancerModel } from "../Models/Freelancer.model";
import { ClientModel } from "../Models/Client.model";

@Injectable({providedIn: "root"})
export class AuthenticationService {
    private baseUrl: string;
    private authDetails = new BehaviorSubject<ClientModel | FreelancerModel | null>(null);
    user$ = this.authDetails.asObservable();

    constructor(private http: HttpClient) {
        this.baseUrl = environment.baseUrl;
    }

    login(email: string, password: string) {
        return this.http.post<ApiResponse<AuthenticationModel>>(
            `${this.baseUrl}/auth/login`, { email, password }
        ).pipe(
            tap((response) => {
                if (response.success) {
                    sessionStorage.setItem('accessToken', response.data.token);
                    this.getMe().subscribe();
                }
            })
        );
    }

    logout() {
        return this.http.post<ApiResponse<string>>(`${this.baseUrl}/auth/logout`, {}).pipe(
            tap(() => {
                sessionStorage.removeItem('accessToken');
                this.authDetails.next(null);
            })
        );
    }

    refreshToken() {
        return this.http.post<ApiResponse<AuthenticationModel>>(`${this.baseUrl}/auth/refresh`, {}, { withCredentials: true })
        .pipe(
            tap((response) => {
                if (response.success) {
                    sessionStorage.setItem('accessToken', response.data.token);
                    this.getMe().subscribe();
                }
            })
        );
    }

    getMe() {
        return this.http.get<ApiResponse<FreelancerModel | ClientModel>>(`${this.baseUrl}/auth/me`)
        .pipe(
            tap((response) => {
                if (response.success) {
                    this.authDetails.next(response.data);
                }else {
                    this.authDetails.next(null);
                }
                }, error => {
                this.authDetails.next(null);
                })
        );
    }
}