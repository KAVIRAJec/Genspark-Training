import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-pagination',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './pagination.html',
  styleUrl: './pagination.css'
})
export class PaginationComponent {
  @Input() page = 1;
  @Input() pageSize = 10;
  @Input() total = 0;
  @Input() siblingCount = 1;
  @Output() pageChange = new EventEmitter<number>();

  get totalPages() {
    return Math.max(1, Math.ceil(this.total / this.pageSize));
  }

  get pages(): number[] {
    const total = this.totalPages;
    const current = this.page;
    const siblings = this.siblingCount;
    const range = [];
    let start = Math.max(1, current - siblings);
    let end = Math.min(total, current + siblings);
    if (start === 1) end = Math.min(total, start + siblings * 2);
    if (end === total) start = Math.max(1, end - siblings * 2);
    for (let i = start; i <= end; i++) range.push(i);
    return range;
  }

  goTo(page: number) {
    if (page < 1 || page > this.totalPages || page === this.page) return;
    this.pageChange.emit(page);
  }
}
