import { Component, HostListener, OnInit } from '@angular/core';
import { ProductModel } from '../Models/product';
import { ProductService } from '../services/product.service';
import { Product } from '../product/product';
import { CartItem } from '../Models/cartItem';
import { FormsModule } from '@angular/forms';
import { debounce, debounceTime, distinctUntilChanged, Subject, switchMap, tap } from 'rxjs';

@Component({
  selector: 'app-products',
  imports: [Product, FormsModule],
  templateUrl: './products.html',
  styleUrl: './products.css'
})
export class Products implements OnInit {
  products: ProductModel[] = [];
  cartItems:CartItem[] =[];
  cartCount:number =0;
  searchString: string = '';
  searchSubject = new Subject<string>();
  loading: boolean = false;
  limit =10;
  skip = 0;
  total = 0;
  constructor(private productService: ProductService) { }

  handleSearchProducts(){
    // console.log(this.searchString)
    this.searchSubject.next(this.searchString);
  }
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
    // this.productService.getAllProducts().subscribe(
    //   {
    //     next: (data: any) => {
    //       this.products = data.products as ProductModel[];
    //     },
    //     error: (error) => {
    //       console.error("Error loading products:", error);
    //     },
    //     complete: () => {
    //       console.log("Products data loaded successfully");
    //     }
    //   }
    // )
    this.searchSubject.pipe(
      debounceTime(3000),
      distinctUntilChanged(),
      tap(()=>this.loading = true),
      switchMap(query => this.productService.getProductSearchResult(query)),
      tap(()=>this.loading = false)
    ).subscribe({
        next:(data:any)=>{
          this.products = data.products as ProductModel[];
          this.total = data.total;
          console.log(this.total)
        }
      });
    }

      @HostListener('window:scroll', [])
      onScroll(): void
      {
        const scrollPosition = window.innerHeight + window.scrollY;
        const threshold = document.body.offsetHeight - 100;
        if(scrollPosition>=threshold && this.products?.length<this.total)
        {
          console.log(scrollPosition);
          console.log(threshold)
          
          this.loadMore();
        }
      }
      loadMore() {
        const prevScrollHeight = document.body.scrollHeight;
        const prevScrollY = window.scrollY;

        this.loading = true;
        this.skip += this.limit;

        this.productService.getProductSearchResult(this.searchString, this.limit, this.skip)
          .subscribe({
            next: (data: any) => {
              this.products = [...this.products, ...data.products];
              this.loading = false;

              setTimeout(() => {
                const newScrollHeight = document.body.scrollHeight;
                const heightDiff = newScrollHeight - prevScrollHeight;
                window.scrollTo(0, heightDiff);
              }, 0); 
            }
          });
      }
}
