
import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PaginationComponent } from '../pagination/pagination';
import { ToastService } from '../Toast/toast.service';
import { ModelService } from '../../services/model.service';
import { ModelDto, CreateModelDto, UpdateModelDto } from '../../models/model.model';
import * as XLSX from 'xlsx';

@Component({
  selector: 'app-model-table',
  standalone: true,
  imports: [CommonModule, FormsModule, PaginationComponent],
  templateUrl: './model-table.component.html',
  styleUrls: ['./model-table.component.css']
})
export class ModelTableComponent implements OnInit {
  models = signal<ModelDto[]>([]);
  loading = signal(false);
  showModal = signal(false);
  editMode = signal(false);
  modalModel = signal<Partial<CreateModelDto & UpdateModelDto>>({});
  selectedModelId = signal<number|null>(null);
  // Search, sort, pagination state
  searchTerm = signal('');
  sortKey = signal<'name'|'createdDate'|'productCount'>('name');
  sortDir = signal<'asc'|'desc'>('asc');
  page = signal(1);
  pageSize = signal(10);

  constructor(private modelService: ModelService, private toast: ToastService) {}

  filteredModels = computed(() => {
    let list = this.models();
    const search = this.searchTerm().trim().toLowerCase();
    if (search) {
      list = list.filter((m: any) =>
        (m.name && m.name.toLowerCase().includes(search))
      );
    }
    return list;
  });

  sortedModels = computed(() => {
    const key = this.sortKey();
    const dir = this.sortDir();
    return [...this.filteredModels()].sort((a: any, b: any) => {
      let v1 = a[key] ?? '';
      let v2 = b[key] ?? '';
      if (typeof v1 === 'string') v1 = v1.toLowerCase();
      if (typeof v2 === 'string') v2 = v2.toLowerCase();
      if (v1 < v2) return dir === 'asc' ? -1 : 1;
      if (v1 > v2) return dir === 'asc' ? 1 : -1;
      return 0;
    });
  });

  pagedModels = computed(() => {
    const start = (this.page() - 1) * this.pageSize();
    return this.sortedModels().slice(start, start + this.pageSize());
  });

  totalModelsCount = computed(() => this.filteredModels().length);
  totalPages = computed(() => Math.max(1, Math.ceil(this.totalModelsCount() / this.pageSize())));

  onSearchChange(term: any) {
    if (typeof term === 'string') {
      this.searchTerm.set(term);
    } else if (term && term.target) {
      this.searchTerm.set(term.target.value);
    }
    this.page.set(1);
  }

  onSort(key: 'name'|'createdDate'|'productCount') {
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

  downloadModelsReport() {
    const data = this.sortedModels();
    if (!data || !data.length) {
      this.toast.show('No models to export.', 'error');
      return;
    }
    const exportData = data.map((m: any) => ({
      'Model Name': m.name,
      'Created': m.createdDate,
      'Product Count': m.productCount
    }));
    const ws: XLSX.WorkSheet = XLSX.utils.json_to_sheet(exportData);
    const wb: XLSX.WorkBook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, 'Models');
    XLSX.writeFile(wb, 'models_report.xlsx');
    this.toast.show('Models report downloaded!', 'success');
  }

  loadAll() {
    this.loading.set(true);
    this.modelService.getAll().subscribe({
      next: res => { this.models.set(res.data || []); this.loading.set(false); },
      error: () => { this.loading.set(false); }
    });
  }

  openCreateModal() {
    this.editMode.set(false);
    this.selectedModelId.set(null);
    this.modalModel.set({ name: '' });
    this.showModal.set(true);
  }

  openEditModal(model: ModelDto) {
    this.editMode.set(true);
    this.selectedModelId.set(model.modelId);
    this.modalModel.set({ ...model });
    this.showModal.set(true);
  }

  closeModal() {
    this.showModal.set(false);
    this.selectedModelId.set(null);
    this.modalModel.set({});
  }

  saveModel() {
    const data = { ...this.modalModel() };
    this.loading.set(true);
    if (this.editMode()) {
      this.modelService.update(this.selectedModelId()!, data as UpdateModelDto).subscribe({
        next: () => {
          this.loadAll();
          this.closeModal();
          this.toast.show('Model updated successfully!', 'success');
        },
        error: () => {
          this.loading.set(false);
          this.toast.show('Failed to update model.', 'error');
        }
      });
    } else {
      this.modelService.create(data as CreateModelDto).subscribe({
        next: () => {
          this.loadAll();
          this.closeModal();
          this.toast.show('Model created successfully!', 'success');
        },
        error: () => {
          this.loading.set(false);
          this.toast.show('Failed to create model.', 'error');
        }
      });
    }
  }

  // Delete modal state
  showDeleteModal = false;
  deleteModelId: number|null = null;
  deleteModelName: string = '';

  openDeleteModal(model: any) {
    this.deleteModelId = model.modelId;
    this.deleteModelName = model.name;
    this.showDeleteModal = true;
  }

  closeDeleteModal() {
    this.showDeleteModal = false;
    this.deleteModelId = null;
    this.deleteModelName = '';
  }

  confirmDeleteModel() {
    if (!this.deleteModelId) return;
    this.loading.set(true);
    this.modelService.delete(this.deleteModelId).subscribe({
      next: () => {
        this.loadAll();
        this.closeDeleteModal();
        this.toast.show('Model deleted successfully!', 'success');
      },
      error: () => {
        this.loading.set(false);
        this.closeDeleteModal();
        this.toast.show('Failed to delete model.', 'error');
      }
    });
  }
}
