import { ProductDto } from './product.model';

export interface CartItem {
  product: ProductDto;
  quantity: number;
}

export interface Cart {
  items: CartItem[];
}
