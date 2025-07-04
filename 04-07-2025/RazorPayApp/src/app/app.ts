import { Component } from '@angular/core';
import { PaymentComponent } from './payment/payment';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [PaymentComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'RazorPayApp';
}
