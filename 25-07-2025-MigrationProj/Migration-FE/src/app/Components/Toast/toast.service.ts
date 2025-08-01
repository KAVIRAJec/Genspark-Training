import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ToastService {
  toast$ = new BehaviorSubject<{ message: string; type: 'success' | 'error' } | null>(null);

  show(message: string, type: 'success' | 'error' = 'success') {
    this.toast$.next({ message, type });
    setTimeout(() => this.toast$.next(null), 3500);
  }
}
