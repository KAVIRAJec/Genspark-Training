import { Routes } from '@angular/router';
import { Home } from './home/home';
import { UserManage } from './user-manage/user-manage';

export const routes: Routes = [
    { path: 'home', component: Home },
    { path: 'userManage', component: UserManage }
];
