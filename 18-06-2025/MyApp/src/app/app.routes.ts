import { Routes } from '@angular/router';
import { First } from './first/first';
import { Products } from './products/products';
import { Login } from './login/login';
import { Home } from './home/home';
import { Profile } from './profile/profile';
import { AuthGuard } from './authguard-guard';

export const routes: Routes = [
    {path: 'landing', component: First},
    {path: 'login', component: Login},
    {path: 'products', component: Products},
    {path: 'home/:un', component: Home,children:
        [
            {path: 'products', component: Products},
            {path: 'first', component: First}
        ]
    },
    {path: 'profile', component: Profile, canActivate: [AuthGuard]}
];
