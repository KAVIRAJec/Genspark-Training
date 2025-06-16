import { Component, HostListener } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ProductModel } from '../Models/Product';
import { debounceTime, distinctUntilChanged, Subject, switchMap, tap } from 'rxjs';
import { ProductService } from '../services/product.service';
import { Product } from "../product/product";

@Component({
  selector: 'app-home',
  imports: [FormsModule, Product],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home {
  products : ProductModel[] = [];
  searchString: string = '';
  searchSubject = new Subject<string>();
  loading: boolean = false;
  limit =10;
  skip = 0;
  total = 0;

  handleSearchProducts() {
    this.searchSubject.next(this.searchString);
  }

  scrollToTop() {
    window.scrollTo({
      top: 0,
      behavior: 'smooth'
    });
  }

  constructor(private productService: ProductService) {}

  ngOnInit() {
    this.searchSubject.pipe(
      debounceTime(3000),
      distinctUntilChanged(),
      tap(() => {
        this.loading = true;
      }),
      switchMap((searchString) => this.productService.getProducts(searchString, this.limit, this.skip)),
      tap(() => this.loading = false)
    ).subscribe({
      next: (data: any) => {
        this.products = data.products as ProductModel[];
        this.total = data.total;
        this.skip = data.products.length;
      },
      error: (error) => {
        console.error("Error loading products:", error);
      },
      complete: () => {
        console.log("Products data loaded successfully");
        this.loading = false;
      }
    });

    this.fetchInitialProducts();
  }

  fetchInitialProducts() {
    this.loading = true;
    this.productService.getProducts(this.searchString, this.limit, this.skip).subscribe({
      next: (data: any) => {
        this.products = [...this.products, ...data.products];
        this.total = data.total;
        this.skip += data.products.length;
        this.loading = false;
      },
      error: () => this.loading = false
    });
  }

  @HostListener('window:scroll', [])
  onScroll(): void {
    const scrollTop = window.scrollY || document.documentElement.scrollTop;
    const scrollHeight = document.documentElement.scrollHeight;
    const clientHeight = window.innerHeight;

    if (!this.loading && scrollTop + clientHeight >= scrollHeight - 100) {
      this.loadMore();
    }
  }

  loadMore() {
    const prevScrollY = window.scrollY;

    this.loading = true;

    this.productService.getProducts(this.searchString, this.limit, this.skip)
      .subscribe({
        next: (data: any) => {
          this.products = [...this.products, ...data.products];
          this.total = data.total;
          this.skip += data.products.length;
          this.loading = false;

          setTimeout(() => {
            window.scrollTo(0, prevScrollY + 100);
          }, 100);
        },
        error: (error) => {
          console.error("Error loading more products:", error);
          this.loading = false;
        }
      });
  }

}
