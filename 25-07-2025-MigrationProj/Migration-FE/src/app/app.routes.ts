import { Routes } from '@angular/router';

import { AuthGuard } from './guards/auth.guard';
import { AdminGuard } from './guards/admin.guard';


import { Authentication } from './Pages/authentication/authentication';
import { Home } from './Pages/home/home';
import { Products } from './Pages/products/products';
import { Cart } from './Pages/cart/cart';
import { ContactUs } from './Pages/contact-us/contact-us';
import { News } from './Pages/news/news';
import { Orders } from './Pages/orders/orders';

// Admin Components
import { ContactUs as AdminContactUs } from './Pages/Admin/contact-us/contact-us';
import { News as AdminNews } from './Pages/Admin/news/news';
import { Orders as AdminOrders } from './Pages/Admin/orders/orders';
import { Products as AdminProducts } from './Pages/Admin/products/products';

export const routes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: 'login', component: Authentication },
  { path: 'register', component: Authentication },
  { path: 'home', component: Home},
  { path: 'products', component: Products},
  { path: 'news', component: News},
  { path: 'contact-us', component: ContactUs, canActivate: [AuthGuard]},
  { path: 'cart', component: Cart, canActivate: [AuthGuard]},
  { path: 'order', component: Orders, canActivate: [AuthGuard]},

  // Admin Routes
  { path: 'admin/products', component: AdminProducts, canActivate: [AuthGuard, AdminGuard] },
  { path: 'admin/news', component: AdminNews, canActivate: [AuthGuard, AdminGuard] },
  { path: 'admin/contact-us', component: AdminContactUs, canActivate: [AuthGuard, AdminGuard] },
  { path: 'admin/orders', component: AdminOrders, canActivate: [AuthGuard, AdminGuard] },
];
