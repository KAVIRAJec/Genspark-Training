import { Component } from '@angular/core';
import { Product } from "./product/product";
import { Products } from "./products/products";
import { Menu } from "./menu/menu";
import { Login } from "./login/login";

@Component({
  selector: 'app-root',
  imports: [Menu, Login],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'MyApp';
}
