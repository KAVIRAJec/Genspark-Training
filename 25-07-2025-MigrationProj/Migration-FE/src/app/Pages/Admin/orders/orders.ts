import { Component, signal, computed, inject, OnInit } from '@angular/core';
import { CommonModule, DatePipe, CurrencyPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { OrdersStatsComponent } from '../../../Components/orders-stats/orders-stats';
import { PaginationComponent } from '../../../Components/pagination/pagination';
import { OrderService } from '../../../services/order.service';
import { ToastService } from '../../../Components/Toast/toast.service';
import { OrderDto } from '../../../models/models';


@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [CommonModule, FormsModule, OrdersStatsComponent, PaginationComponent, DatePipe, CurrencyPipe],
  templateUrl: './orders.html',
  styleUrls: ['./orders.css']
})
export class Orders implements OnInit {
  async downloadOrdersExcel() {
    const XLSX = await import('xlsx');
    // Use sortedOrders() so export matches current filter/sort
    const data = this.sortedOrders().map(o => ({
      'Order #': o.orderId,
      'User': o.userName || o.userId,
      'Date': o.orderDate,
      'Status': o.status,
      'Total': o.totalAmount,
      'Items': o.orderDetails?.length ?? 0,
      'Shipping Address': o.shippingAddress,
      'Notes': o.notes
    }));
    const ws = XLSX.utils.json_to_sheet(data);
    const wb = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, 'Orders');
    XLSX.writeFile(wb, `orders-report-${new Date().toISOString().slice(0,10)}.xlsx`);
  }
  orders = signal<OrderDto[]>([]);
  loading = signal(false);

  showDetailModal = signal(false);
  detailOrder = signal<OrderDto | null>(null);

  // Search, sort, pagination state
  searchTerm = signal('');
  sortKey = signal<'orderId'|'userName'|'orderDate'|'status'|'totalAmount'>('orderDate');
  sortDir = signal<'asc'|'desc'>('desc');
  page = signal(1);
  pageSize = signal(10);

  filteredOrders = computed(() => {
    let list = this.orders();
    const search = this.searchTerm().trim().toLowerCase();
    if (search) {
      list = list.filter(o =>
        (''+o.orderId).includes(search) ||
        (o.userName && o.userName.toLowerCase().includes(search)) ||
        (o.status && o.status.toLowerCase().includes(search))
      );
    }
    return list;
  });

  sortedOrders = computed(() => {
    const key = this.sortKey();
    const dir = this.sortDir();
    return [...this.filteredOrders()].sort((a, b) => {
      let v1 = a[key] ?? '';
      let v2 = b[key] ?? '';
      if (key === 'orderDate') {
        v1 = new Date(v1).getTime();
        v2 = new Date(v2).getTime();
      }
      if (typeof v1 === 'string') v1 = v1.toLowerCase();
      if (typeof v2 === 'string') v2 = v2.toLowerCase();
      if (v1 < v2) return dir === 'asc' ? -1 : 1;
      if (v1 > v2) return dir === 'asc' ? 1 : -1;
      return 0;
    });
  });

  pagedOrders = computed(() => {
    const start = (this.page() - 1) * this.pageSize();
    return this.sortedOrders().slice(start, start + this.pageSize());
  });

  totalOrdersCount = computed(() => this.filteredOrders().length);
  totalPages = computed(() => Math.max(1, Math.ceil(this.totalOrdersCount() / this.pageSize())));

  onSearchChange(term: any) {
    // Handles both string and event
    if (typeof term === 'string') {
      this.searchTerm.set(term);
    } else if (term && term.target) {
      this.searchTerm.set(term.target.value);
    }
    this.page.set(1);
  }

  onSort(key: 'orderId'|'userName'|'orderDate'|'status'|'totalAmount') {
    if (this.sortKey() === key) {
      this.sortDir.set(this.sortDir() === 'asc' ? 'desc' : 'asc');
    } else {
      this.sortKey.set(key);
      this.sortDir.set('asc');
    }
    this.page.set(1);
  }

  onPageChange(page: any) {
    // Handles both number and event
    if (typeof page === 'number') {
      this.page.set(page);
    } else if (page && page.target && !isNaN(+page.target.value)) {
      this.page.set(+page.target.value);
    }
  }

  constructor(private orderService: OrderService, private toastService: ToastService) {}

  ngOnInit() {
    this.fetchOrders();
  }

  fetchOrders() {
    this.orderService.getAll().subscribe({
      next: (res) => {
        this.orders.set(res.data || []);
      },
      error: (err) => {
        this.toastService.show('Failed to load orders.', 'error');
      },
      complete: () => {
        this.loading.set(false);
      }
    });
  }

  openOrderModal(order: OrderDto) {
    this.detailOrder.set(order);
    this.showDetailModal.set(true);
  }

  closeOrderModal() {
    this.showDetailModal.set(false);
    this.detailOrder.set(null);
  }

  totalOrders = computed(() => this.orders().length);
  totalProducts = computed(() => this.orders().reduce((sum, o) => sum + (o.orderDetails?.length || 0), 0));
  totalQuantity = computed(() => this.orders().reduce((sum, o) => sum + (o.orderDetails?.reduce((s, d) => s + (d.quantity || 0), 0) || 0), 0));
  delivered = computed(() => this.orders().filter(o => o.status === 'Delivered').length);
  cancelled = computed(() => this.orders().filter(o => o.status === 'Cancelled').length);
  pending = computed(() => this.orders().filter(o => o.status === 'Pending').length);

  thisMonthOrders = computed(() => {
    const now = new Date();
    return this.orders().filter(o => {
      const d = new Date(o.orderDate);
      return d.getMonth() === now.getMonth() && d.getFullYear() === now.getFullYear();
    }).length;
  });

  thisMonthRevenue = computed(() => {
    const now = new Date();
    return this.orders().filter(o => {
      const d = new Date(o.orderDate);
      return d.getMonth() === now.getMonth() && d.getFullYear() === now.getFullYear();
    }).reduce((sum, o) => sum + (o.totalAmount || 0), 0);
  });
}
