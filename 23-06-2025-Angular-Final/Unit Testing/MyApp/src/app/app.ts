import { Component } from '@angular/core';
import { Product } from "./product/product";
import { Products } from "./products/products";
import { Menu } from "./menu/menu";
import { Login } from "./login/login";
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  imports: [Menu, RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'MyApp';
}
