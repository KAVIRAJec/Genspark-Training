import { Component, Input } from '@angular/core';
import { ProductModel } from '../Models/Product';
import { CurrencyPipe } from '@angular/common';
import { ProductService } from '../services/product.service';

@Component({
  selector: 'app-product',
  imports: [CurrencyPipe],
  templateUrl: './product.html',
  styleUrl: './product.css'
})
export class Product {
@Input() Product: ProductModel | null = new ProductModel();

constructor(private productService: ProductService) {}
  addToCart() {
    try {
      const data = this.productService.addToCart(this.Product);
      if (Array.isArray(data) && data.length > 0) {
        console.log('Product added to cart:', data[0].title);
      } else {
        console.log('Product added to cart:', data);
      }
    } catch (error) {
      console.error('Error adding product to cart:', error);
    }
  }
}
