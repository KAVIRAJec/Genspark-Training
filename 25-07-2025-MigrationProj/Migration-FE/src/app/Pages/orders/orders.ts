import { Component, signal, effect, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrderService } from '../../services/order.service';
import { OrderDto } from '../../models/order.model';
import { AuthService } from '../../services/auth.service';
import { PaginationComponent } from '../../Components/pagination/pagination';

@Component({
  selector: 'app-orders',
  imports: [CommonModule, PaginationComponent],
  templateUrl: './orders.html',
  styleUrl: './orders.css'
})
export class Orders implements OnInit{
  orders = signal<OrderDto[]>([]);
  loading = signal(true);
  userId = signal<number | null>(null);
  page = signal(1);
  pageSize = signal(5);

  pagedOrders = computed(() => {
    const all = this.orders();
    const start = (this.page() - 1) * this.pageSize();
    return all.slice(start, start + this.pageSize());
  });

  onPageChange(page: number) {
    this.page.set(page);
  }

  ngOnInit(): void {
    this.authService.getCurrentUser().subscribe({
      next: (res) => {
        if (res && res.data && res.data.userId) {
          this.userId.set(res.data.userId);
        }
      },
      error: () => {
        this.userId.set(null);
      }
    });
  }

  constructor(private orderService: OrderService, private authService: AuthService) {
    effect(() => {
      this.loading.set(true);
      this.orderService.getByUserId(this.userId() as number).subscribe({
        next: (res) => {
          this.orders.set(res.data || []);
          this.loading.set(false);
        },
        error: () => {
          this.orders.set([]);
          this.loading.set(false);
        }
      });
    });
  }
}
