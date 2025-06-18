import { Routes } from '@angular/router';
import { Home } from './home/home';
import { Login } from './login/login';
import { authguardGuard } from './authguard-guard';
import { Users } from './users/users';

export const routes: Routes = [
    {path: '', redirectTo: '/home', pathMatch: 'full'},
    {path: 'home', component: Home, canActivate: [authguardGuard]},
    {path: 'users', component: Users, canActivate: [authguardGuard]},
    {path: 'login', component: Login },
];
