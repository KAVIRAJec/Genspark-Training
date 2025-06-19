import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { ProductModel } from '../Models/product';
import { ProductService } from '../services/product.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-product',
  imports: [CommonModule],
  templateUrl: './product.html',
  styleUrl: './product.css'
})
export class Product {
  @Input() Product:ProductModel| null = new ProductModel();
  @Output() addToCart:EventEmitter<number> = new EventEmitter<number>();
  private productService = inject(ProductService);

  handleBuyClick(pid:number|undefined){
  if(pid)
  {
      this.addToCart.emit(pid);
  }
}

  constructor() {
    // this.productService.getProducts().subscribe(
    //   {
    //     next: (data) => {
    //       this.Product = data as ProductModel;
    //       console.log(this.Product);
    //     },
    //     error: (error) => {
    //       console.log(error);
    //     },
    //     complete: () => {
    //       console.log("Product data loaded successfully");
    //     }
    //   }
    // )
    

  }
}
