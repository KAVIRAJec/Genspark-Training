import { Routes } from '@angular/router';
import { Login } from './login/login';
import { Profile } from './profile/profile';

export const routes: Routes = [
   { path: '', component: Login },
  { path: 'profile', component: Profile }
];

