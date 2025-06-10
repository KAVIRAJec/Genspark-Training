import { Component } from '@angular/core';
import { CustomerDetailsModel } from './models'; 
import customerData from './customers.json';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-customer-details',
  imports: [CommonModule],
  templateUrl: './customer-details.html',
  styleUrl: './customer-details.css'
})
export class CustomerDetails {
  customers: CustomerDetailsModel[] = customerData;
}
