import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProductDto } from '../../models/models';

@Component({
  selector: 'app-product-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './product-card.html',
  styleUrl: './product-card.css'
})
export class ProductCardComponent {
  @Input() product!: ProductDto;
  @Output() addToCart = new EventEmitter<ProductDto>();

  onAddToCart() {
    this.addToCart.emit(this.product);
  }

}
