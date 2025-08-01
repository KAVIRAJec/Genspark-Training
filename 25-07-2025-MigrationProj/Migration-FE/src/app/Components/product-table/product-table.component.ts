import { Component, OnInit, signal, computed } from '@angular/core';
import { ToastService } from '../Toast/toast.service';
import { CommonModule } from '@angular/common';
import { ProductService } from '../../services/product.service';
import { ProductDto, CreateProductDto, UpdateProductDto } from '../../models/product.model';
import { CategoryService } from '../../services/category.service';
import { ColorService } from '../../services/color.service';
import { ModelService } from '../../services/model.service';
import { FormsModule } from '@angular/forms';
import { PaginationComponent } from '../pagination/pagination';
import * as XLSX from 'xlsx';
import { AuthService } from '../../services/auth.service';

@Component({
    selector: 'app-product-table',
    imports: [CommonModule, FormsModule, PaginationComponent],
    templateUrl: './product-table.component.html',
    styleUrls: ['./product-table.component.css']
})
export class ProductTableComponent implements OnInit {
    products = signal<ProductDto[]>([]);
    loading = signal(false);
    showModal = signal(false);
    editMode = signal(false);
    modalProduct = signal<Partial<CreateProductDto & UpdateProductDto>>({});
    selectedProductId = signal<number|null>(null);
    categories = signal<any[]>([]);
    colors = signal<any[]>([]);
    models = signal<any[]>([]);
    showProductDetailModal = signal(false);
    selectedProductDetail = signal<ProductDto | null>(null);
    userId: number | undefined;

    constructor(
        private productService: ProductService,
        private authService: AuthService,
        private categoryService: CategoryService,
        private colorService: ColorService,
        private modelService: ModelService,
        private toast: ToastService
    ) {}
    
    // Search, sort, pagination state
    searchTerm = signal('');
    sortKey = signal<'productName'|'categoryName'|'colorName'|'modelName'|'price'|'isActive'>('productName');
    sortDir = signal<'asc'|'desc'>('asc');
    page = signal(1);
    pageSize = signal(10);
    
    filteredProducts = computed(() => {
        let list = this.products();
        const search = this.searchTerm().trim().toLowerCase();
        if (search) {
            list = list.filter((p: any) =>
                (p.productName && p.productName.toLowerCase().includes(search)) ||
            (p.categoryName && p.categoryName.toLowerCase().includes(search)) ||
            (p.colorName && p.colorName.toLowerCase().includes(search)) ||
            (p.modelName && p.modelName.toLowerCase().includes(search))
        );
    }
    return list;
});

sortedProducts = computed(() => {
    const key = this.sortKey();
    const dir = this.sortDir();
    return [...this.filteredProducts()].sort((a: any, b: any) => {
        let v1 = a[key] ?? '';
        let v2 = b[key] ?? '';
          if (typeof v1 === 'string') v1 = v1.toLowerCase();
          if (typeof v2 === 'string') v2 = v2.toLowerCase();
          if (v1 < v2) return dir === 'asc' ? -1 : 1;
          if (v1 > v2) return dir === 'asc' ? 1 : -1;
          return 0;
        });
    });
    
      pagedProducts = computed(() => {
        const start = (this.page() - 1) * this.pageSize();
        return this.sortedProducts().slice(start, start + this.pageSize());
      });
    
      totalProductsCount = computed(() => this.filteredProducts().length);
      totalPages = computed(() => Math.max(1, Math.ceil(this.totalProductsCount() / this.pageSize())));
    
      onSearchChange(term: any) {
        if (typeof term === 'string') {
          this.searchTerm.set(term);
        } else if (term && term.target) {
          this.searchTerm.set(term.target.value);
        }
        this.page.set(1);
      }
    
      onSort(key: 'productName'|'categoryName'|'colorName'|'modelName'|'price'|'isActive') {
        if (this.sortKey() === key) {
          this.sortDir.set(this.sortDir() === 'asc' ? 'desc' : 'asc');
        } else {
          this.sortKey.set(key);
          this.sortDir.set('asc');
        }
        this.page.set(1);
      }
    
      onPageChange(page: any) {
        if (typeof page === 'number') {
          this.page.set(page);
        } else if (page && page.target && !isNaN(+page.target.value)) {
          this.page.set(+page.target.value);
        }
      }
    
    ngOnInit() {
        this.loadAll();
        this.categoryService.getAll().subscribe({ next: (res: any) => this.categories.set(res.data || []), error: () => {} });
        this.colorService.getAll().subscribe({ next: (res: any) => this.colors.set(res.data || []), error: () => {} });
        this.modelService.getAll().subscribe({ next: (res: any) => this.models.set(res.data || []), error: () => {} });
        // Fetch userId from /me endpoint
        const token = localStorage.getItem('accessToken');
        if (token) {
            // Decode JWT to extract userId
            const payload = JSON.parse(atob(token.split('.')[1]));
            console.log('Decoded payload:', payload);
            this.userId = payload.UserId || payload.id || payload.sub;
        }
    }

    downloadProductsReport() {
    const data = this.sortedProducts();
    if (!data || !data.length) {
      (this.toast as any).show('No products to export.', 'info');
      return;
    }
    // Prepare data for Excel
    const exportData = data.map((p: any) => ({
      'Product Name': p.productName,
      'Category': p.categoryName,
      'Color': p.colorName,
      'Model': p.modelName,
      'Price': p.price,
    }));
    const ws: XLSX.WorkSheet = XLSX.utils.json_to_sheet(exportData);
    const wb: XLSX.WorkBook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, 'Products');
    XLSX.writeFile(wb, 'products_report.xlsx');
    (this.toast as any).show('Products report downloaded!', 'success');
  }

  loadAll() {
    this.loading.set(true);
    this.productService.getAll().subscribe({
      next: res => { this.products.set(res.data || []); this.loading.set(false); },
      error: () => { this.loading.set(false); }
    });
  }

  openCreateModal() {
    this.editMode.set(false);
    this.selectedProductId.set(null);
    this.modalProduct.set({ productName: '', price: 0, isActive: true });
    this.showModal.set(true);
  }

  openEditModal(product: ProductDto) {
    this.editMode.set(true);
    this.selectedProductId.set(product.productId);
    this.modalProduct.set({ ...product });
    this.showModal.set(true);
  }

  closeModal() {
    this.showModal.set(false);
    this.selectedProductId.set(null);
    this.modalProduct.set({});
  }

    saveProduct() {
        const data = { ...this.modalProduct() };
        data.userId = this.userId;
        console.log('Saving product:', data, this.userId);
        if (data.sellStartDate) {
            data.sellStartDate = new Date(data.sellStartDate).toISOString();
        }
        if (data.sellEndDate) {
            data.sellEndDate = new Date(data.sellEndDate).toISOString();
        }
        this.loading.set(true);
        if (this.editMode()) {
            this.productService.update(this.selectedProductId()!, data as UpdateProductDto).subscribe({
                next: () => {
                    this.loadAll();
                    this.closeModal();
                    this.toast.show('Product updated successfully!', 'success');
                },
                error: () => {
                    this.loading.set(false);
                    this.toast.show('Failed to update product.', 'error');
                }
            });
        } else {
            this.productService.create(data as CreateProductDto).subscribe({
                next: () => {
                    this.loadAll();
                    this.closeModal();
                    this.toast.show('Product created successfully!', 'success');
                },
                error: () => {
                    this.loading.set(false);
                    this.toast.show('Failed to create product.', 'error');
                }
            });
        }
    }


  // Delete modal state
  showDeleteModal = false;
  deleteProductId: number|null = null;
  deleteProductName: string = '';

  openDeleteModal(product: any) {
    this.deleteProductId = product.productId;
    this.deleteProductName = product.productName;
    this.showDeleteModal = true;
  }

  closeDeleteModal() {
    this.showDeleteModal = false;
    this.deleteProductId = null;
    this.deleteProductName = '';
  }

  confirmDeleteProduct() {
    if (!this.deleteProductId) return;
    this.loading.set(true);
    this.productService.delete(this.deleteProductId).subscribe({
      next: () => {
        this.loadAll();
        this.closeDeleteModal();
        this.toast.show('Product deleted successfully!', 'success');
      },
      error: () => {
        this.loading.set(false);
        this.closeDeleteModal();
        this.toast.show('Failed to delete product.', 'error');
      }
    });
  }
  openProductDetailModal(product: ProductDto) {
        this.selectedProductDetail.set(product);
        this.showProductDetailModal.set(true);
    }

    closeProductDetailModal() {
        this.showProductDetailModal.set(false);
        this.selectedProductDetail.set(null);
    }
}
