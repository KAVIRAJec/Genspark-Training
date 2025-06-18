import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { UserLoginModel, UserModel } from "../Models/User";
import { BehaviorSubject, Observable } from "rxjs";
import { map } from 'rxjs/operators';

@Injectable()
export class UserService {
private currentUserSubject: BehaviorSubject<string | null> = new BehaviorSubject<string | null>(null);
public currentUser$: Observable<string | null> = this.currentUserSubject.asObservable();

 baseUrl = 'https://dummyjson.com';
    
 constructor(private http: HttpClient) {}
  
 loginUser(user: UserLoginModel) {
    if (user.username && user.password) {
      var response = this.http.post(`${this.baseUrl}/auth/login`, user);
      response.subscribe({
        next: (data: any) => {
          if (data && data.accessToken) {
            localStorage.setItem('accessToken', data.accessToken);
            this.currentUserSubject.next(data.username);
          }
        },
        error: (error) => {
          console.error('Login failed', error);
        }
      });
    }
    return null;
  }
 GetCurrentUser(): Observable<any | null> {
    const accessToken = localStorage.getItem('accessToken');
    if (accessToken) {
      var response = this.http.get(`${this.baseUrl}/users/me`, {
        headers: {
          Authorization: `Bearer ${accessToken}`
        }
      });
      response.subscribe({
        next: (data: any) => {
          this.currentUserSubject.next(data.username);
        },
        error: (error) => {
          console.error('Failed to fetch current user', error);
        }
      });
      return response;
    }
    return new BehaviorSubject<any | null>(null).asObservable();
  }
 logOutUser() {
    localStorage.removeItem('accessToken');
    this.currentUserSubject.next(null);
 }
 getAllUsers(): Observable<UserModel[]> {
  return this.http.get<any>(`${this.baseUrl}/users`).pipe(
    map(response => {
      return (response.users || []).map((user: any) => ({
        id: user.id,
        firstName: user.firstName,
        lastName: user.lastName,
        email: user.email,
        password: user.password,
        phone: user.phone,
        gender: user.gender,
        age: user.age,
        address: user.address?.address || '',
        city: user.address?.city || '',
        state: user.address?.state || '',
        postalCode: user.address?.postalCode || '',
        country: user.address?.country || '',
        role: user.role
      }));
    })
  );
 }
 getUserById(id: number): Observable<UserModel> { 
    return this.http.get<any>(`${this.baseUrl}/users/${id}`).pipe(
    map(response => {
      var user: UserModel;
      user = {
        id: response.id,
        firstName: response.firstName,
        lastName: response.lastName,
        email: response.email,
        password: response.password,
        phone: response.phone,
        gender: response.gender,
        age: response.age,
        address: response.address?.address || '',
        city: response.address?.city || '',
        state: response.address?.state || '',
        postalCode: response.address?.postalCode || '',
        country: response.address?.country || '',
        role: response.role
      };
      return user;
    })
  );
 }
updateUser() { }
deleteUser() { }
createUser(data: UserModel): Observable<UserModel> {
    return this.http.post<UserModel>(`${this.baseUrl}/users/add`, data).pipe(
      map(response => {
        console.log('User created successfully:', response);
        return {
          id: response.id,
          firstName: response.firstName,
          lastName: response.lastName,
          email: response.email,
          password: response.password,
          phone: response.phone,
          age: response.age,
          gender: response.gender,
          address: response.address,
          city: '',
          state: '',
          postalCode: '',
          country: '',
          role: response.role
        };
      })
    );
  }
}
