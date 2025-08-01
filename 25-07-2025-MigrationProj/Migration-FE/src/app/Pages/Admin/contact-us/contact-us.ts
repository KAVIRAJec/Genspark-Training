import { Component, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ContactUsService } from '../../../services/contact-us.service';
import { ToastService } from '../../../Components/Toast/toast.service';
import { ContactUsDto } from '../../../models/contact-us.model';
import { UpdateContactUsDto } from '../../../models/models';
import { PaginationComponent } from "../../../Components/pagination/pagination";

@Component({
  selector: 'app-contact-us',
  imports: [CommonModule, FormsModule, PaginationComponent],
  templateUrl: './contact-us.html',
  styleUrls: ['./contact-us.css']
})
export class ContactUs implements OnInit {
  async downloadContactUsExcel() {
    const XLSX = await import('xlsx');
    // Use sortedQueries() so export matches current filter/sort
    const data = this.sortedQueries().map(q => ({
      'ID': q.contactId,
      'Name': q.name,
      'Email': q.email,
      'Subject': q.subject,
      'Message': q.message,
      'Created Date': q.createdDate,
      'Is Read': q.isRead ? 'Yes' : 'No',
      'Is Active': q.isActive ? 'Yes' : 'No',
      'Response': q.response,
      'Response Date': q.responseDate
    }));
    const ws = XLSX.utils.json_to_sheet(data);
    const wb = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, 'ContactUs');
    XLSX.writeFile(wb, `contact-us-report-${new Date().toISOString().slice(0,10)}.xlsx`);
  }
  // Search, sort, pagination state for queries
  searchTerm = signal('');
  sortKey = signal<'createdDate'|'name'|'email'|'subject'|'isRead'>('createdDate');
  sortDir = signal<'asc'|'desc'>('desc');
  page = signal(1);
  pageSize = signal(10);

  filteredQueries = computed(() => {
    let list = this.queries();
    const search = this.searchTerm().trim().toLowerCase();
    if (search) {
      list = list.filter(q =>
        (q.name && q.name.toLowerCase().includes(search)) ||
        (q.email && q.email.toLowerCase().includes(search)) ||
        (q.subject && q.subject.toLowerCase().includes(search)) ||
        (q.message && q.message.toLowerCase().includes(search))
      );
    }
    return list;
  });

  sortedQueries = computed(() => {
    const key = this.sortKey();
    const dir = this.sortDir();
    return [...this.filteredQueries()].sort((a, b) => {
      let v1 = a[key] ?? '';
      let v2 = b[key] ?? '';
      if (key === 'createdDate') {
        v1 = typeof v1 === 'string' ? new Date(v1).toISOString() : '';
        v2 = typeof v2 === 'string' ? new Date(v2).toISOString() : '';
      } else if (typeof v1 === 'boolean' && typeof v2 === 'boolean') {
        v1 = v1 ? '1' : '0';
        v2 = v2 ? '1' : '0';
      } else {
        if (typeof v1 === 'string') v1 = v1.toLowerCase();
        if (typeof v2 === 'string') v2 = v2.toLowerCase();
      }
      if (v1 < v2) return dir === 'asc' ? -1 : 1;
      if (v1 > v2) return dir === 'asc' ? 1 : -1;
      return 0;
    });
  });

  pagedQueries = computed(() => {
    const start = (this.page() - 1) * this.pageSize();
    return this.sortedQueries().slice(start, start + this.pageSize());
  });

  totalQueriesCount = computed(() => this.filteredQueries().length);
  totalPages = computed(() => Math.max(1, Math.ceil(this.totalQueriesCount() / this.pageSize())));

  onSearchChange(term: any) {
    if (typeof term === 'string') {
      this.searchTerm.set(term);
    } else if (term && term.target) {
      this.searchTerm.set(term.target.value);
    }
    this.page.set(1);
  }

  onResponseChange(term: any) {
    if (typeof term === 'string') {
      this.responseText.set(term);
    } else if (term && term.target) {
      this.responseText.set(term.target.value);
    }
    this.page.set(1);
  }

  onSort(key: 'createdDate'|'name'|'email'|'subject'|'isRead') {
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
  queries = signal<ContactUsDto[]>([]);
  loading = signal(false);
  error = signal<string|null>(null);
  success = signal<string|null>(null);

  showModal = signal(false);
  selectedQuery = signal<ContactUsDto|null>(null);
  editMode = signal(false);
  responseText = signal<string>('');

  constructor(private contactService: ContactUsService, private toast: ToastService) {}

  ngOnInit() {
    this.loadQueries();
  }

  loadQueries() {
    this.loading.set(true);
    this.error.set(null);
    this.contactService.getAll().subscribe({
      next: res => {
        this.queries.set(res.data || []);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load queries');
        this.toast.show('Failed to load queries', 'error');
        this.loading.set(false);
      }
    });
  }

  openModal(query: ContactUsDto, edit = false) {
    this.selectedQuery.set(query);
    this.editMode.set(edit);
    this.responseText.set(query.response || '');
    this.showModal.set(true);
  }

  closeModal() {
    this.showModal.set(false);
    this.selectedQuery.set(null);
    this.editMode.set(false);
    this.responseText.set('');
  }

  saveResponse() {
    const q = this.selectedQuery();
    if (!q) return;
    this.loading.set(true);
    let updated = {
      contactId: q.contactId,
      name: q.name,
      email: q.email,
      subject: q.subject,
      message: q.message,
      isRead: true,
      response: this.responseText(),
      responseDate: new Date().toISOString()
    };
    console.log('Saving response for query:', this.responseText());
    this.contactService.update(q.contactId, updated).subscribe({
      next: res => {
        this.toast.show('Response saved', 'success');
        this.loadQueries();
        this.closeModal();
      },
      error: () => {
        this.toast.show('Failed to save response', 'error');
        this.loading.set(false);
      }
    });
  }

  deleteQuery() {
    const q = this.selectedQuery();
    if (!q) return;
    this.loading.set(true);
    this.contactService.delete(q.contactId).subscribe({
      next: () => {
        this.toast.show('Query deleted', 'success');
        this.loadQueries();
        this.closeModal();
      },
      error: () => {
        this.toast.show('Failed to delete query', 'error');
        this.loading.set(false);
      }
    });
  }
}
