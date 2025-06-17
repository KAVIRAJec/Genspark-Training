import { Component } from '@angular/core';
import { ProductService } from '../services/product.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-cart',
  imports: [CommonModule],
  templateUrl: './cart.html',
  styleUrl: './cart.css'
})
export class Cart {
  aboutInfo: any;
  cart: any[] = [];

  constructor(private productService: ProductService) {}

  ngOnInit() {
    this.aboutInfo = this.productService.getAboutInfo();
    this.cart = this.productService.getCart();
  }

  addToCart(product: any) {
    this.cart = this.productService.addToCart(product);
  }

  removeFromCart(product: any) {
    this.cart = this.productService.removeFromCart(product);
  }
}
