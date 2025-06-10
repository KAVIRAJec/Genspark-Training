import { Component } from '@angular/core';
import CustomerData from './CustomerDetails.json';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-customer-rating',
  imports: [CommonModule],
  templateUrl: './customer-rating.html',
  styleUrl: './customer-rating.css'
})
export class CustomerRating {
Customers = CustomerData;
  constructor(){
    for (let customer of this.Customers) {
      var likes = customer.Likes;
      var dislikes = customer.Dislikes;
      customer.Rating = (likes / (likes + dislikes)) * 5;
      customer.Rating = Math.round(customer.Rating * 5) / 5;
    }
  }

  AddLikes(customerId: number) {
    const customer = this.Customers.find(c => c.customer_id === (customerId));
    if (customer) {
      customer.Likes++;
      customer.Rating = (customer.Likes / (customer.Likes + customer.Dislikes)) * 5;
      customer.Rating = Math.round(customer.Rating * 5) / 5;
    }
  }

  AddDislikes(customerId: number) {
    const customer = this.Customers.find(c => c.customer_id === (customerId));
    if (customer) {
      customer.Dislikes++;
      customer.Rating = (customer.Likes / (customer.Likes + customer.Dislikes)) * 5;
      customer.Rating = Math.round(customer.Rating * 5) / 5;
    }
  }
}
