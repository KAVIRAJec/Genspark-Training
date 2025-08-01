import { Component, signal, effect, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NewsService } from '../../services/news.service';
import type { NewsDto } from '../../models/news.model';
import { NewsCardComponent } from '../../Components/news-card/news-card';
import { PaginationComponent } from '../../Components/pagination/pagination';

@Component({
  selector: 'app-news',
  imports: [CommonModule, NewsCardComponent, PaginationComponent],
  templateUrl: './news.html',
  styleUrl: './news.css'
})
export class News {
  newsList = signal<NewsDto[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);

    // Modal state for selected news
  showModal = signal(false);
  modalNews = signal<NewsDto|null>(null);

  // Search/sort/pagination signals
  search = signal('');
  sortBy = signal<'date-desc'|'date-asc'|'title-asc'|'title-desc'|''>('');
  page = signal(1);
  pageSize = signal(9);

  // Computed filtered news
  filteredNews = computed(() => {
    let list = this.newsList();
    const search = this.search().toLowerCase();
    if (search) {
      list = list.filter(n => n.title?.toLowerCase().includes(search) || n.content?.toLowerCase().includes(search));
    }
    switch (this.sortBy()) {
      case 'date-desc':
        list = [...list].sort((a, b) => new Date(b.createdDate).getTime() - new Date(a.createdDate).getTime());
        break;
      case 'date-asc':
        list = [...list].sort((a, b) => new Date(a.createdDate).getTime() - new Date(b.createdDate).getTime());
        break;
      case 'title-asc':
        list = [...list].sort((a, b) => (a.title ?? '').localeCompare(b.title ?? ''));
        break;
      case 'title-desc':
        list = [...list].sort((a, b) => (b.title ?? '').localeCompare(a.title ?? ''));
        break;
    }
    return list;
  });

  pagedNews = computed(() => {
    const list = this.filteredNews();
    const start = (this.page() - 1) * this.pageSize();
    return list.slice(start, start + this.pageSize());
  });

  constructor(private newsService: NewsService) {
    effect(() => {
      this.page.set(1);
      this.filteredNews();
    });
    effect(() => {
      this.loading.set(true);
      this.newsService.getPublished().subscribe({
        next: (res) => {
          this.newsList.set(res.data || []);
          this.loading.set(false);
        },
        error: () => {
          this.error.set('Failed to load news.');
          this.loading.set(false);
        }
      });
    });
  }

  onReadMore(news: NewsDto) {
    this.modalNews.set(news);
    this.showModal.set(true);
  }

  closeModal() {
    this.showModal.set(false);
    this.modalNews.set(null);
  }

  onSearchInput(event: Event) {
    const value = (event.target && (event.target as HTMLInputElement).value) || '';
    this.search.set(value);
  }
  onSortChange(event: Event) {
    const value = (event.target && (event.target as HTMLSelectElement).value) || '';
    this.sortBy.set(value as any);
  }
  onPageChange(page: number) {
    this.page.set(page);
  }
}
