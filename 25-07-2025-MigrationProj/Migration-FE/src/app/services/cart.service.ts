import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { Cart, CartItem } from '../../../../Migration-FE/src/app/models/cart.model';

const CART_KEY = 'shopping_cart';

@Injectable({ providedIn: 'root' })
export class CartService {
  private cartSubject: BehaviorSubject<Cart>;
  loading = new BehaviorSubject<boolean>(false);
  success = new BehaviorSubject<string | null>(null);
  error = new BehaviorSubject<string | null>(null);

  constructor() {
    const storedCart = localStorage.getItem(CART_KEY);
    this.cartSubject = new BehaviorSubject<Cart>(storedCart ? JSON.parse(storedCart) : { items: [] });
  }

  getCart(): Observable<Cart> {
    return this.cartSubject.asObservable();
  }

  addItem(item: CartItem): void {
    this.loading.next(true);
    this.success.next(null);
    this.error.next(null);
    try {
      const cart = this.cartSubject.value;
      const index = cart.items.findIndex(i => i.product.productId === item.product.productId);
      if (index > -1) {
        cart.items[index].quantity += item.quantity;
      } else {
        cart.items.push(item);
      }
      this.updateCart(cart);
      this.success.next(index > -1 ? 'Updated cart item.' : 'Added to cart.');
    } catch (e) {
      this.error.next('Failed to add to cart.');
    } finally {
      this.loading.next(false);
    }
  }

  removeItem(productId: number): void {
    this.loading.next(true);
    this.success.next(null);
    this.error.next(null);
    try {
      const cart = this.cartSubject.value;
      cart.items = cart.items.filter(i => i.product.productId !== productId);
      this.updateCart(cart);
      this.success.next('Removed from cart.');
    } catch (e) {
      this.error.next('Failed to remove from cart.');
    } finally {
      this.loading.next(false);
    }
  }

  clearCart(): void {
    this.loading.next(true);
    this.success.next(null);
    this.error.next(null);
    try {
      this.updateCart({ items: [] });
      this.success.next('Cart cleared.');
    } catch (e) {
      this.error.next('Failed to clear cart.');
    } finally {
      this.loading.next(false);
    }
  }

  private updateCart(cart: Cart): void {
    localStorage.setItem(CART_KEY, JSON.stringify(cart));
    this.cartSubject.next(cart);
  }
}
