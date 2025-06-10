import { Component } from '@angular/core';
import ProductData from './ProductDetails.json';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-product-cart',
  imports: [CommonModule],
  templateUrl: './product-cart.html',
  styleUrl: './product-cart.css'
})
export class ProductCart {
  products = ProductData;
  cartItems: { [productId: string]: number } = {};

  addToCart(productId: string) {
    if (this.cartItems[productId]) {
      this.cartItems[productId]++;
    } else {
      this.cartItems[productId] = 1;
    }
  }

  removeFromCart(productId: string) {
    if (this.cartItems[productId]) {
      this.cartItems[productId]--;
      if (this.cartItems[productId] === 0) {
        delete this.cartItems[productId];
      }
    }
  }

}
