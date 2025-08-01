import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-toast',
  imports: [CommonModule],
  template: `
    <div *ngIf="show" [ngClass]="toastClass" class="fixed top-6 right-6 z-50 px-6 py-4 rounded shadow-lg flex items-center gap-2 animate-fade-in">
      <span>{{ message }}</span>
      <button (click)="show = false" class="ml-4 text-lg font-bold">&times;</button>
    </div>
  `,
  styles: [`
    .animate-fade-in { animation: fadeIn 0.3s; }
    @keyframes fadeIn { from { opacity: 0; transform: translateY(-10px); } to { opacity: 1; transform: none; } }
    .toast-success { background: #d1fae5; color: #065f46; }
    .toast-error { background: #fee2e2; color: #991b1b; }
  `]
})
export class ToastComponent {
  @Input() message = '';
  @Input() type: 'success' | 'error' = 'success';
  show = true;

  get toastClass() {
    return this.type === 'success' ? 'toast-success' : 'toast-error';
  }
}
