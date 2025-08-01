import { Component, signal, effect, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ContactUsService } from '../../services/contact-us.service';
import { ToastService } from '../../Components/Toast/toast.service';
import { CreateContactUsDto, ContactUsDto } from '../../models/contact-us.model';
import { PaginationComponent } from '../../Components/pagination/pagination';
import AOS from 'aos';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-contact-us',
  imports: [CommonModule, FormsModule, PaginationComponent],
  templateUrl: './contact-us.html',
  styleUrl: './contact-us.css'
})
export class ContactUs {
  // Form signals
  userId = signal<number|null>(null);
  name = signal('');
  email = signal('');
  subject = signal('');
  message = signal('');

  // State signals
  loading = signal(false);
  error = signal<string|null>(null);
  success = signal<string|null>(null);

  // Query list signals
  queries = signal<ContactUsDto[]>([]);
  page = signal(1);
  pageSize = signal(5);
  pagedQueries = computed(() => {
    const all = this.queries();
    const start = (this.page() - 1) * this.pageSize();
    return all.slice(start, start + this.pageSize());
  });

  constructor(private contactService: ContactUsService, private authService: AuthService, private toast: ToastService) {
    this.authService.getCurrentUser().subscribe({
      next: (res) => {
        if(res?.data && res.data?.userName && res.data?.email) {
          this.name.set(res.data.userName);
          this.email.set(res.data.email);
          this.userId.set(res.data.userId);
          this.loadQueries();
        }
      },
      error: () => {
        console.error('Failed to fetch user data');
        this.toast.show('Failed to fetch user details', 'error');
      }
    })
  }

  loadQueries() {
    this.loading.set(true);
    this.contactService.getByUserId(this.userId()!).subscribe({
      next: (res) => {
        this.queries.set(res.data || []);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set('Failed to load your queries.');
        this.loading.set(false);
      }
    });
  }

  submit() {
    if (!this.name() || !this.email() || !this.message()) {
      this.toast.show('Name, Email, and Message are required.', 'error');
      return;
    }
    this.loading.set(true);
    const dto: CreateContactUsDto = {
      name: this.name(),
      email: this.email(),
      subject: this.subject(),
      message: this.message()
    };
    this.contactService.sendMessage(dto).subscribe({
      next: (res) => {
        this.toast.show('Query submitted successfully!', 'success');
        this.name.set('');
        this.email.set('');
        this.subject.set('');
        this.message.set('');
        this.loadQueries();
        this.loading.set(false);
      },
      error: (err) => {
        this.toast.show('Failed to submit query.', 'error');
        this.loading.set(false);
      }
    });
  }

  onPageChange(page: number) {
    this.page.set(page);
  }
}
