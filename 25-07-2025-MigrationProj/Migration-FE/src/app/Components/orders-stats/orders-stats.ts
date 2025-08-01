import { CommonModule, CurrencyPipe } from '@angular/common';
import { Component, Input, signal, computed } from '@angular/core';

@Component({
  selector: 'app-orders-stats',
  imports: [CommonModule, CurrencyPipe],
  templateUrl: './orders-stats.html',
  styleUrls: ['./orders-stats.css']
})
export class OrdersStatsComponent {
  @Input() totalOrders: number = 0;
  @Input() totalProducts: number = 0;
  @Input() totalQuantity: number = 0;
  @Input() delivered: number = 0;
  @Input() cancelled: number = 0;
  @Input() pending: number = 0;
  @Input() thisMonthOrders: number = 0;
  @Input() thisMonthRevenue: number = 0;

  // Use @Input values directly in template
  get deliveredPercent() {
    return this.totalOrders ? Math.round((this.delivered / this.totalOrders) * 100) : 0;
  }
}
