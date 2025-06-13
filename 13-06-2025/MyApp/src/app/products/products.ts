import { Component, OnInit } from '@angular/core';
import { ProductModel } from '../Models/product';
import { ProductService } from '../services/product.service';
import { Product } from '../product/product';
import { CartItem } from '../Models/cartItem';

@Component({
  selector: 'app-products',
  imports: [Product],
  templateUrl: './products.html',
  styleUrl: './products.css'
})
export class Products implements OnInit {
  products: ProductModel[] | undefined = undefined;
  cartItems:CartItem[] =[];
  cartCount:number =0;
  constructor(private productService: ProductService) { }
  handleAddToCart(event: number)
  {
    console.log("Handling add to cart - "+event)
    let flag = false;
    for(let i=0;i<this.cartItems.length;i++)
    {
      if(this.cartItems[i].Id==event)
      {
         this.cartItems[i].Count++;
         flag=true;
      }
    }
    if(!flag)
      this.cartItems.push(new CartItem(event,1));
    this.cartCount++;
  }

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
