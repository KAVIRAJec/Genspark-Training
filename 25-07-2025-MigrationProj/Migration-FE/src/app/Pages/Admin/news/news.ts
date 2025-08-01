import { Component, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NewsService } from '../../../services/news.service';
import { NewsDto, CreateNewsDto, UpdateNewsDto } from '../../../models/news.model';
import { ToastService } from '../../../Components/Toast/toast.service';
import { PaginationComponent } from '../../../Components/pagination/pagination';

@Component({
  selector: 'app-news',
  imports: [CommonModule, FormsModule, PaginationComponent
  ],
  templateUrl: './news.html',
  styleUrl: './news.css'
})
export class News {
  async downloadNewsExcel() {
    const XLSX = await import('xlsx');
    // Use filteredNews() so export matches current filter/search
    const data = this.filteredNews().map(n => ({
      'ID': n.newsId,
      'Title': n.title,
      'Summary': n.summary,
      'Content': n.content,
      'Author': n.authorName,
      'Published': n.isPublished ? 'Yes' : 'No',
      'Active': n.isActive ? 'Yes' : 'No',
      'Created Date': n.createdDate,
      'Updated Date': n.updatedDate
    }));
    const ws = XLSX.utils.json_to_sheet(data);
    const wb = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, 'News');
    XLSX.writeFile(wb, `news-report-${new Date().toISOString().slice(0,10)}.xlsx`);
  }
  
  newsList = signal<NewsDto[]>([]);
  loading = signal(false);
  error = signal<string|null>(null);
  success = signal<string|null>(null);
  // Modal state
  showModal = signal(false);
  modalMode = signal<'create'|'edit'>('create');
  modalTitle = signal('');
  modalNews = signal<any>({});
  selectedNewsId = signal<number|null>(null);

  // News detail modal state
  showDetailModal = false;
  detailNews: NewsDto | null = null;

  // Search, filter, pagination signals
  searchTerm = signal('');
  filterPublished = signal(''); // '', 'published', 'unpublished'
  page = signal(1);
  pageSize = signal(8);

  // Computed for filtered and paged news
  filteredNews = computed(() => {
    let list = this.newsList();
    const search = this.searchTerm().trim().toLowerCase();
    if (search) {
      list = list.filter(n =>
        n.title.toLowerCase().includes(search) ||
        (n.summary && n.summary.toLowerCase().includes(search)) ||
        (n.content && n.content.toLowerCase().includes(search)) ||
        (n.authorName && n.authorName.toLowerCase().includes(search))
      );
    }
    if (this.filterPublished() === 'published') {
      list = list.filter(n => n.isPublished);
    } else if (this.filterPublished() === 'unpublished') {
      list = list.filter(n => !n.isPublished);
    }
    return list;
  });

  totalNews = computed(() => this.filteredNews().length);
  totalPages = computed(() => Math.max(1, Math.ceil(this.totalNews() / this.pageSize())));
  pagedNews = computed(() => {
    const start = (this.page() - 1) * this.pageSize();
    return this.filteredNews().slice(start, start + this.pageSize());
  });

  constructor(private newsService: NewsService, private toast: ToastService) {
    this.loadAll();
  }

  openDetailModal(news: NewsDto) {
    this.detailNews = news;
    this.showDetailModal = true;
  }

  closeDetailModal() {
    this.showDetailModal = false;
    this.detailNews = null;
  }

  loadAll() {
    this.loading.set(true);
    this.error.set(null);
    this.success.set(null);
    this.newsService.getAll().subscribe({
      next: res => {
        this.newsList.set(res.data || []);
        this.page.set(1); // Reset to first page on reload
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load news');
        this.loading.set(false);
      }
    });
  }

  onSearchChange(term: string) {
    this.searchTerm.set(term);
    this.page.set(1);
  }

  onFilterChange(val: string) {
    this.filterPublished.set(val);
    this.page.set(1);
  }

  onPageChange(page: number) {
    this.page.set(page);
  }

  openModal(mode: 'create'|'edit', news?: NewsDto) {
    this.modalMode.set(mode);
    this.modalTitle.set(mode === 'create' ? 'Create News' : 'Edit News');
    if (mode === 'edit' && news) {
      this.modalNews.set({ ...news });
      this.selectedNewsId.set(news.newsId);
    } else {
      this.modalNews.set({ title: '', content: '', summary: '', image: '', isPublished: false, authorId: 1 });
      this.selectedNewsId.set(null);
    }
    this.showModal.set(true);
  }

  closeModal() {
    this.showModal.set(false);
    this.selectedNewsId.set(null);
  }

  saveNews(news: any) {
    if (!news.title || !news.content) {
      this.error.set('Title and content are required');
      this.success.set(null);
      return;
    }
    this.loading.set(true);
    this.error.set(null);
    this.success.set(null);
    if (this.modalMode() === 'create') {
      this.newsService.create(news as CreateNewsDto).subscribe({
        next: () => {
          this.success.set('News created successfully');
          this.error.set(null);
          this.toast.show('News created successfully', 'success');
          this.loadAll();
          this.closeModal();
          this.loading.set(false);
        },
        error: () => {
          this.error.set('Failed to create news');
          this.success.set(null);
          this.toast.show('Failed to create news', 'error');
          this.loading.set(false);
        }
      });
    } else if (this.modalMode() === 'edit' && this.selectedNewsId()) {
      this.newsService.update(this.selectedNewsId()!, news as UpdateNewsDto).subscribe({
        next: () => {
          this.success.set('News updated successfully');
          this.error.set(null);
          this.toast.show('News updated successfully', 'success');
          this.loadAll();
          this.closeModal();
          this.loading.set(false);
        },
        error: () => {
          this.error.set('Failed to update news');
          this.success.set(null);
          this.toast.show('Failed to update news', 'error');
          this.loading.set(false);
        }
      });
    }
  }

  deleteNews(id: number) {
    if (!confirm('Delete this news item?')) return;
    this.loading.set(true);
    this.error.set(null);
    this.success.set(null);
    this.newsService.delete(id).subscribe({
      next: () => {
        this.success.set('News deleted successfully');
        this.error.set(null);
        this.toast.show('News deleted successfully', 'success');
        this.loadAll();
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to delete news');
        this.success.set(null);
        this.toast.show('Failed to delete news', 'error');
        this.loading.set(false);
      }
    });
  }

  publishNews(id: number) {
    this.loading.set(true);
    this.error.set(null);
    this.success.set(null);
    this.newsService.publish(id).subscribe({
      next: () => {
        this.success.set('News published successfully');
        this.error.set(null);
        this.toast.show('News published successfully', 'success');
        this.loadAll();
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to publish news');
        this.success.set(null);
        this.toast.show('Failed to publish news', 'error');
        this.loading.set(false);
      }
    });
  }

  unpublishNews(id: number) {
    this.loading.set(true);
    this.error.set(null);
    this.success.set(null);
    this.newsService.unpublish(id).subscribe({
      next: () => {
        this.success.set('News unpublished successfully');
        this.error.set(null);
        this.toast.show('News unpublished successfully', 'success');
        this.loadAll();
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to unpublish news');
        this.success.set(null);
        this.toast.show('Failed to unpublish news', 'error');
        this.loading.set(false);
      }
    });
  }
}
