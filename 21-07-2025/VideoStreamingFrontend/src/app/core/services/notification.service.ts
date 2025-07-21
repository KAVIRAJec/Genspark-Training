import { Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  constructor(private toastr: ToastrService) {}

  showSuccess(message: string, title: string = 'Success'): void {
    this.toastr.success(message, title, {
      timeOut: 5000,
      progressBar: true,
      closeButton: true,
      enableHtml: true
    });
  }

  showError(message: string, title: string = 'Error'): void {
    this.toastr.error(message, title, {
      timeOut: 8000,
      progressBar: true,
      closeButton: true,
      enableHtml: true
    });
  }

  showWarning(message: string, title: string = 'Warning'): void {
    this.toastr.warning(message, title, {
      timeOut: 6000,
      progressBar: true,
      closeButton: true,
      enableHtml: true
    });
  }

  showInfo(message: string, title: string = 'Info'): void {
    this.toastr.info(message, title, {
      timeOut: 5000,
      progressBar: true,
      closeButton: true,
      enableHtml: true
    });
  }
}
