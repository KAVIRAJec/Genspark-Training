import { Component, OnInit } from '@angular/core';
import { ProductModel } from '../Models/product';
import { ProductService } from '../services/product.service';
import { Product } from '../product/product';

@Component({
  selector: 'app-products',
  imports: [Product],
  templateUrl: './products.html',
  styleUrl: './products.css'
})
export class Products implements OnInit {
  products: ProductModel[] | undefined = undefined;
  constructor(private productService: ProductService) { }

  ngOnInit(): void {
    this.productService.getAllProducts().subscribe(
      {
        next: (data: any) => {
          this.products = data.products as ProductModel[];
        },
        error: (error) => {
          console.error("Error loading products:", error);
        },
        complete: () => {
          console.log("Products data loaded successfully");
        }
      }
    )
  }

}
