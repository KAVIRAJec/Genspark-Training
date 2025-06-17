import { Routes } from '@angular/router';
import { Home } from './home/home';
import { Products } from './products/products';
import { Product } from './product/product';
import { AuthGuard } from './authguard-guard';
import { Login } from './login/login';
import { Cart } from './cart/cart';
import { ProductDetails } from './product-details/product-details';

export const routes: Routes = [
    {path: '', redirectTo: 'home', pathMatch: 'full'},
    {path: 'home', component: Home},
    {path: 'cart', component: Cart, canActivate: [AuthGuard]},
    {path: 'products', component: Products, canActivate: [AuthGuard], 
        children: 
        [
            {path: ':id', component: Product}
        ]
    },
    {path: 'product/:id', component: ProductDetails, canActivate: [AuthGuard]},
    {path: 'login', component: Login}
];
