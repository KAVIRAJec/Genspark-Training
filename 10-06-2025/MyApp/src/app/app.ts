import { Component } from '@angular/core';
import { First } from './first/first';
import { CustomerDetails } from "./customer-details/customer-details";
import { ProductCart } from "./product-cart/product-cart";
import { CustomerRating } from "./customer-rating/customer-rating";

@Component({
  selector: 'app-root',
  imports: [First, CustomerDetails, ProductCart, CustomerRating],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'MyApp';
}
