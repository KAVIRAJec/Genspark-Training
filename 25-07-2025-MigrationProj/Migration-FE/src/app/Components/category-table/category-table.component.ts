import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PaginationComponent } from '../pagination/pagination';
import { ToastService } from '../Toast/toast.service';
import { CategoryService } from '../../services/category.service';
import { CategoryDto } from '../../models/category.model';
import * as XLSX from 'xlsx';

@Component({
  selector: 'app-category-table',
  standalone: true,
  imports: [CommonModule, FormsModule, PaginationComponent],
  templateUrl: './category-table.component.html',
  styleUrls: ['./category-table.component.css']
})
export class CategoryTableComponent implements OnInit {
  categories = signal<CategoryDto[]>([]);
  loading = signal(false);
  showModal = signal(false);
  editMode = signal(false);
  modalCategory = signal<Partial<CategoryDto>>({});
  selectedCategoryId = signal<number|null>(null);
  // Search, sort, pagination state
  searchTerm = signal('');
  sortKey = signal<'name'|'productCount'|'createdDate'>('name');
  sortDir = signal<'asc'|'desc'>('asc');
  page = signal(1);
  pageSize = signal(10);

  // Delete modal state
  showDeleteModal = false;
  deleteCategoryId: number|null = null;
  deleteCategoryName: string = '';

  constructor(private categoryService: CategoryService, private toast: ToastService) {}

  filteredCategories = computed(() => {
    let list = this.categories();
    const search = this.searchTerm().trim().toLowerCase();
    if (search) {
      list = list.filter((c: any) =>
        (c.name && c.name.toLowerCase().includes(search)) ||
        (c.createdDate && new Date(c.createdDate).toLocaleDateString().toLowerCase().includes(search))
      );
    }
    return list;
  });

  sortedCategories = computed(() => {
    const key = this.sortKey();
    const dir = this.sortDir();
    return [...this.filteredCategories()].sort((a: any, b: any) => {
      let v1 = a[key] ?? '';
      let v2 = b[key] ?? '';
      if (typeof v1 === 'string') v1 = v1.toLowerCase();
      if (typeof v2 === 'string') v2 = v2.toLowerCase();
      if (v1 < v2) return dir === 'asc' ? -1 : 1;
      if (v1 > v2) return dir === 'asc' ? 1 : -1;
      return 0;
    });
  });

  pagedCategories = computed(() => {
    const start = (this.page() - 1) * this.pageSize();
    return this.sortedCategories().slice(start, start + this.pageSize());
  });

  totalCategoriesCount = computed(() => this.filteredCategories().length);
  totalPages = computed(() => Math.max(1, Math.ceil(this.totalCategoriesCount() / this.pageSize())));

  onSearchChange(term: any) {
    if (typeof term === 'string') {
      this.searchTerm.set(term);
    } else if (term && term.target) {
      this.searchTerm.set(term.target.value);
    }
    this.page.set(1);
  }

  onSort(key: 'name'|'productCount') {
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
  }

  downloadCategoriesReport() {
    const data = this.sortedCategories();
    if (!data || !data.length) {
      this.toast.show('No categories to export.', 'error');
      return;
    }
    const exportData = data.map((c: any) => ({
      'Category Name': c.name,
      'Product Count': c.productCount,
      'Created': c.createdDate ? new Date(c.createdDate).toLocaleDateString() : ''
    }));
    const ws: XLSX.WorkSheet = XLSX.utils.json_to_sheet(exportData);
    const wb: XLSX.WorkBook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, 'Categories');
    XLSX.writeFile(wb, 'categories_report.xlsx');
    this.toast.show('Categories report downloaded!', 'success');
  }

  loadAll() {
    this.loading.set(true);
    this.categoryService.getAll().subscribe({
      next: res => { this.categories.set(res.data || []); this.loading.set(false); },
      error: () => { this.loading.set(false); }
    });
  }

  openCreateModal() {
    this.editMode.set(false);
    this.selectedCategoryId.set(null);
    this.modalCategory.set({ name: '' });
    this.showModal.set(true);
  }

  openEditModal(category: CategoryDto) {
    this.editMode.set(true);
    this.selectedCategoryId.set(category.categoryId);
    this.modalCategory.set({ ...category });
    this.showModal.set(true);
  }

  closeModal() {
    this.showModal.set(false);
    this.selectedCategoryId.set(null);
    this.modalCategory.set({});
  }

  saveCategory() {
    const modal = this.modalCategory();
    const name = modal.name ?? '';
    this.loading.set(true);
    if (this.editMode()) {
      const updateData = { categoryId: this.selectedCategoryId()!, name };
      this.categoryService.update(this.selectedCategoryId()!, updateData).subscribe({
        next: () => {
          this.loadAll();
          this.closeModal();
          this.toast.show('Category updated successfully!', 'success');
        },
        error: () => {
          this.loading.set(false);
          this.toast.show('Failed to update category.', 'error');
        }
      });
    } else {
      const createData = { name };
      this.categoryService.create(createData).subscribe({
        next: () => {
          this.loadAll();
          this.closeModal();
          this.toast.show('Category created successfully!', 'success');
        },
        error: () => {
          this.loading.set(false);
          this.toast.show('Failed to create category.', 'error');
        }
      });
    }
  }

  openDeleteModal(category: any) {
    this.deleteCategoryId = category.categoryId;
    this.deleteCategoryName = category.name;
    this.showDeleteModal = true;
  }

  closeDeleteModal() {
    this.showDeleteModal = false;
    this.deleteCategoryId = null;
    this.deleteCategoryName = '';
  }

  confirmDeleteCategory() {
    if (!this.deleteCategoryId) return;
    this.loading.set(true);
    this.categoryService.delete(this.deleteCategoryId).subscribe({
      next: () => {
        this.loadAll();
        this.closeDeleteModal();
        this.toast.show('Category deleted successfully!', 'success');
      },
      error: () => {
        this.loading.set(false);
        this.closeDeleteModal();
        this.toast.show('Failed to delete category.', 'error');
      }
    });
  }
}
