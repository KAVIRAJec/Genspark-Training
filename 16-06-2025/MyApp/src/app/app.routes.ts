import { Routes } from '@angular/router';
import { First } from './first/first';
import { Products } from './products/products';
import { Login } from './login/login';

export const routes: Routes = [
    {path: 'home', component: First},
    {path: 'login', component: Login},
    {path: 'products', component: Products}
];
