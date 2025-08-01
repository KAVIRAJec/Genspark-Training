import { Component, signal, effect, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProductCardComponent } from '../../Components/product-card/product-card';
import { PaginationComponent } from '../../Components/pagination/pagination';
import { ProductService } from '../../services/product.service';
import { CategoryService } from '../../services/category.service';
import { ColorService } from '../../services/color.service';
import { ModelService } from '../../services/model.service';
import { ProductDto } from '../../models/product.model';
import { CartService } from '../../services/cart.service';
import { ToastService } from '../../Components/Toast/toast.service';
import { CategoryDto } from '../../models/category.model';
import { ColorDto } from '../../models/color.model';
import { ModelDto } from '../../models/model.model';

@Component({
  selector: 'app-products',
  imports: [CommonModule, ProductCardComponent, PaginationComponent],
  templateUrl: './products.html',
  styleUrl: './products.css'
})
export class Products {
  products = signal<ProductDto[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);

  // Filter/search/sort signals
  search = signal('');
  selectedCategory = signal<number|null>(null);
  selectedColor = signal<number|null>(null);
  selectedModel = signal<number|null>(null);
  sortBy = signal<'price-asc'|'price-desc'|'name-asc'|'name-desc'|''>('');

  // Filter options
  categories = signal<CategoryDto[]>([]);
  colors = signal<ColorDto[]>([]);
  models = signal<ModelDto[]>([]);

  // Pagination signals
  page = signal(1);
  pageSize = signal(9); // 9 per page for grid

  // Computed filtered products (all matching)
  filteredProducts = computed(() => {
    let list = this.products();
    // Search
    const search = this.search().toLowerCase();
    if (search) {
      list = list.filter(p => p.productName?.toLowerCase().includes(search));
    }
    // Category
    if (this.selectedCategory()) {
      list = list.filter(p => p.categoryId === this.selectedCategory());
    }
    // Color
    if (this.selectedColor()) {
      list = list.filter(p => p.colorId === this.selectedColor());
    }
    // Model
    if (this.selectedModel()) {
      list = list.filter(p => p.modelId === this.selectedModel());
    }
    // Sort
    switch (this.sortBy()) {
      case 'price-asc':
        list = [...list].sort((a, b) => (a.price ?? 0) - (b.price ?? 0));
        break;
      case 'price-desc':
        list = [...list].sort((a, b) => (b.price ?? 0) - (a.price ?? 0));
        break;
      case 'name-asc':
        list = [...list].sort((a, b) => (a.productName ?? '').localeCompare(b.productName ?? ''));
        break;
      case 'name-desc':
        list = [...list].sort((a, b) => (b.productName ?? '').localeCompare(a.productName ?? ''));
        break;
    }
    return list;
  });

  // Computed paged products (for current page)
  pagedProducts = computed(() => {
    const list = this.filteredProducts();
    const start = (this.page() - 1) * this.pageSize();
    return list.slice(start, start + this.pageSize());
  });

  constructor(
    private productService: ProductService,
    private categoryService: CategoryService,
    private colorService: ColorService,
    private modelService: ModelService,
    private cartService: CartService,
    private toastService: ToastService
  ) {
    // Reset page to 1 if filteredProducts length changes
    effect(() => {
      this.page.set(1);
      this.filteredProducts();
    });

    // Load products
    effect(() => {
      this.loading.set(true);
      this.productService.getAll().subscribe({
        next: (res) => {
          this.products.set(res.data || []);
          this.loading.set(false);
        },
        error: (err) => {
          this.error.set('Failed to load products.');
          this.loading.set(false);
        }
      });
    });
    // Load categories
    this.categoryService.getAll().subscribe({
      next: (res) => this.categories.set(res.data || []),
      error: () => this.categories.set([])
    });
    // Load colors
    this.colorService.getAll().subscribe({
      next: (res) => this.colors.set(res.data || []),
      error: () => this.colors.set([])
    });
    // Load models
    this.modelService.getAll().subscribe({
      next: (res) => this.models.set(res.data || []),
      error: () => this.models.set([])
    });
  }

  // Add to cart handler
  onAddToCart(product: ProductDto) {
    this.cartService.addItem({ product, quantity: 1 });
    this.cartService.success.subscribe(msg => {
      if (msg) this.toastService.show(msg, 'success');
    });
    this.cartService.error.subscribe(msg => {
      if (msg) this.toastService.show(msg, 'error');
    });
  }

  // Pagination event handler
  onPageChange(page: number) {
    this.page.set(page);
  }

  // UI event helpers for template (to avoid template type errors)
  onSearchInput(event: Event) {
    const value = (event.target && (event.target as HTMLInputElement).value) || '';
    this.search.set(value);
  }
  onCategoryChange(event: Event) {
    const value = (event.target && (event.target as HTMLSelectElement).value) || '';
    this.selectedCategory.set(value ? +value : null);
  }
  onColorChange(event: Event) {
    const value = (event.target && (event.target as HTMLSelectElement).value) || '';
    this.selectedColor.set(value ? +value : null);
  }
  onModelChange(event: Event) {
    const value = (event.target && (event.target as HTMLSelectElement).value) || '';
    this.selectedModel.set(value ? +value : null);
  }
  onSortChange(event: Event) {
    const value = (event.target && (event.target as HTMLSelectElement).value) || '';
    this.sortBy.set(value as any);
  }
}
