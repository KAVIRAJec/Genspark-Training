
import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PaginationComponent } from '../pagination/pagination';
import { ToastService } from '../Toast/toast.service';
import { ColorService } from '../../services/color.service';
import { ColorDto } from '../../models/color.model';
import * as XLSX from 'xlsx';

@Component({
  selector: 'app-color-table',
  standalone: true,
  imports: [CommonModule, FormsModule, PaginationComponent],
  templateUrl: './color-table.component.html',
  styleUrls: ['./color-table.component.css']
})

export class ColorTableComponent implements OnInit {
  colors = signal<ColorDto[]>([]);
  loading = signal(false);
  showModal = signal(false);
  editMode = signal(false);
  modalColor = signal<Partial<ColorDto>>({});
  selectedColorId = signal<number|null>(null);
  // Search, sort, pagination state
  searchTerm = signal('');
  sortKey = signal<'name'|'hexCode'|'productCount'|'createdDate'>('name');
  sortDir = signal<'asc'|'desc'>('asc');
  page = signal(1);
  pageSize = signal(10);

  // Delete modal state
  showDeleteModal = false;
  deleteColorId: number|null = null;
  deleteColorName: string = '';

  constructor(private colorService: ColorService, private toast: ToastService) {}

  filteredColors = computed(() => {
    let list = this.colors();
    const search = this.searchTerm().trim().toLowerCase();
    if (search) {
      list = list.filter((c: any) =>
        (c.name && c.name.toLowerCase().includes(search)) ||
        (c.hexCode && c.hexCode.toLowerCase().includes(search)) ||
        (c.createdDate && new Date(c.createdDate).toLocaleDateString().toLowerCase().includes(search))
      );
    }
    return list;
  });

  sortedColors = computed(() => {
    const key = this.sortKey();
    const dir = this.sortDir();
    return [...this.filteredColors()].sort((a: any, b: any) => {
      let v1 = a[key] ?? '';
      let v2 = b[key] ?? '';
      if (typeof v1 === 'string') v1 = v1.toLowerCase();
      if (typeof v2 === 'string') v2 = v2.toLowerCase();
      if (v1 < v2) return dir === 'asc' ? -1 : 1;
      if (v1 > v2) return dir === 'asc' ? 1 : -1;
      return 0;
    });
  });

  pagedColors = computed(() => {
    const start = (this.page() - 1) * this.pageSize();
    return this.sortedColors().slice(start, start + this.pageSize());
  });

  totalColorsCount = computed(() => this.filteredColors().length);
  totalPages = computed(() => Math.max(1, Math.ceil(this.totalColorsCount() / this.pageSize())));

  onSearchChange(term: any) {
    if (typeof term === 'string') {
      this.searchTerm.set(term);
    } else if (term && term.target) {
      this.searchTerm.set(term.target.value);
    }
    this.page.set(1);
  }

  onSort(key: 'name'|'hexCode'|'productCount') {
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

  downloadColorsReport() {
    const data = this.sortedColors();
    if (!data || !data.length) {
      this.toast.show('No colors to export.', 'error');
      return;
    }
    const exportData = data.map((c: any) => ({
      'Color Name': c.name,
      'Hex': c.hexCode,
      'Product Count': c.productCount,
      'Created': c.createdDate ? new Date(c.createdDate).toLocaleDateString() : ''
    }));
    const ws: XLSX.WorkSheet = XLSX.utils.json_to_sheet(exportData);
    const wb: XLSX.WorkBook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, 'Colors');
    XLSX.writeFile(wb, 'colors_report.xlsx');
    this.toast.show('Colors report downloaded!', 'success');
  }

  loadAll() {
    this.loading.set(true);
    this.colorService.getAll().subscribe({
      next: res => { this.colors.set(res.data || []); this.loading.set(false); },
      error: () => { this.loading.set(false); }
    });
  }

  openCreateModal() {
    this.editMode.set(false);
    this.selectedColorId.set(null);
    this.modalColor.set({ name: '', hexCode: '' });
    this.showModal.set(true);
  }

  openEditModal(color: ColorDto) {
    this.editMode.set(true);
    this.selectedColorId.set(color.colorId);
    this.modalColor.set({ ...color });
    this.showModal.set(true);
  }

  closeModal() {
    this.showModal.set(false);
    this.selectedColorId.set(null);
    this.modalColor.set({});
  }

  saveColor() {
    const modal = this.modalColor();
    const name = modal.name ?? '';
    const hexCode = modal.hexCode ?? '';
    this.loading.set(true);
    if (this.editMode()) {
      const updateData = { colorId: this.selectedColorId()!, name, hexCode };
      this.colorService.update(this.selectedColorId()!, updateData).subscribe({
        next: () => {
          this.loadAll();
          this.closeModal();
          this.toast.show('Color updated successfully!', 'success');
        },
        error: () => {
          this.loading.set(false);
          this.toast.show('Failed to update color.', 'error');
        }
      });
    } else {
      const createData = { name, hexCode };
      this.colorService.create(createData).subscribe({
        next: () => {
          this.loadAll();
          this.closeModal();
          this.toast.show('Color created successfully!', 'success');
        },
        error: () => {
          this.loading.set(false);
          this.toast.show('Failed to create color.', 'error');
        }
      });
    }
  }

  openDeleteModal(color: any) {
    this.deleteColorId = color.colorId;
    this.deleteColorName = color.name;
    this.showDeleteModal = true;
  }

  closeDeleteModal() {
    this.showDeleteModal = false;
    this.deleteColorId = null;
    this.deleteColorName = '';
  }

  confirmDeleteColor() {
    if (!this.deleteColorId) return;
    this.loading.set(true);
    this.colorService.delete(this.deleteColorId).subscribe({
      next: () => {
        this.loadAll();
        this.closeDeleteModal();
        this.toast.show('Color deleted successfully!', 'success');
      },
      error: () => {
        this.loading.set(false);
        this.closeDeleteModal();
        this.toast.show('Failed to delete color.', 'error');
      }
    });
  }
}
