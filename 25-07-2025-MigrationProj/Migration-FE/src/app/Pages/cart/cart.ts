import { Component, signal, effect, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CartService } from '../../services/cart.service';
import { CartItem } from '../../models/cart.model';
import { ToastService } from '../../Components/Toast/toast.service';
import { OrderService } from '../../services/order.service';
import { AuthService } from '../../services/auth.service';
import { CreateOrderDto } from '../../models/order.model';
import { environment } from '../../../environments/environment';
import { CreateOrderDetailDto } from '../../models/order-detail.model';

@Component({
  selector: 'app-cart',
  imports: [CommonModule, FormsModule],
  templateUrl: './cart.html',
  styleUrl: './cart.css'
})
export class Cart {
  cart = signal({ items: [] as CartItem[] });
  loading = signal(false);
  showCheckout = signal(false);
  shippingAddress = signal('');
  notes = signal('');
  userId: number | null = null;
  paypalPaymentId: string | null = null;
  paypalLoaded = signal(false);
  @ViewChild('paypalButton', { static: false }) paypalButton?: ElementRef;

  constructor(
    private cartService: CartService,
    private toast: ToastService,
    private orderService: OrderService,
    private authService: AuthService
  ) {
    effect(() => {
      this.cartService.getCart().subscribe(cart => this.cart.set(cart));
    });
    // Fetch current user id on init
    this.authService.getCurrentUser().subscribe({
      next: (res) => {
        if (res && res.data && res.data.userId) {
          this.userId = res.data.userId;
        }
      },
      error: () => {
        this.userId = null;
      }
    });
    effect(() => {
      this.cartService.loading.subscribe(l => this.loading.set(l));
      this.cartService.success.subscribe(msg => { if (msg) this.toast.show(msg, 'success'); });
      this.cartService.error.subscribe(msg => { if (msg) this.toast.show(msg, 'error'); });
    });
  }

  get total() {
    return this.cart().items.reduce((sum, item) => sum + (item.product.price || 0) * item.quantity, 0);
  }

  increaseQty(item: CartItem) {
    this.cartService.addItem({ product: item.product, quantity: 1 });
  }
  decreaseQty(item: CartItem) {
    if (item.quantity > 1) {
      this.cartService.addItem({ product: item.product, quantity: -1 });
    }
  }
  removeItem(productId: number) {
    this.cartService.removeItem(productId);
  }
  openCheckout() {
    this.showCheckout.set(true);
  }

  renderPaypalButton() {
    if (this.paypalLoaded()) return;
    const clientId = environment.paypalClientId;
    if (!clientId) {
      this.toast.show('PayPal Client ID not set in environment.', 'error');
      return;
    }
    // Dynamically load PayPal script if not already present
    if (!document.getElementById('paypal-sdk')) {
      const script = document.createElement('script');
      script.id = 'paypal-sdk';
      script.src = `https://www.paypal.com/sdk/js?client-id=${clientId}&currency=USD`;
      script.onload = () => this.initPaypalButton();
      document.body.appendChild(script);
    } else {
      this.initPaypalButton();
    }
  }

  initPaypalButton() {
    this.paypalLoaded.set(true);
    // @ts-ignore
    if (window.paypal && this.paypalButton) {
      // @ts-ignore
      window.paypal.Buttons({
        createOrder: (data: any, actions: any) => {
          return actions.order.create({
            purchase_units: [{ amount: { value: this.total.toFixed(2) } }]
          });
        },
        onApprove: (data: any, actions: any) => {
          return actions.order.capture().then((details: any) => {
            this.paypalPaymentId = details.id;
            this.toast.show('PayPal payment successful!', 'success');
          });
        },
        onError: (err: any) => {
          this.toast.show('PayPal payment failed.', 'error');
        }
      }).render(this.paypalButton.nativeElement);
    }
  }
  onShippingAddressChange(value: string) {
    this.shippingAddress.set(value);
  }
  onNotesChange(value: string) {
    this.notes.set(value);
  }
  closeCheckout() {
    this.showCheckout.set(false);
  }
  confirmCheckout() {
    if (!this.userId) {
      this.toast.show('User not authenticated.', 'error');
      return;
    }
    const address = this.shippingAddress().trim();
    if (!address || address.length < 8 || !/[a-zA-Z]/.test(address) || !/\d/.test(address)) {
      this.toast.show('Please enter a valid shipping address (min 8 chars, include letters & numbers).', 'error');
      return;
    }
    if (!this.paypalPaymentId) {
      this.toast.show('Please complete PayPal payment before confirming.', 'error');
      return;
    }
    const order: CreateOrderDto = {
      userId: this.userId,
      shippingAddress: this.shippingAddress(),
      notes: this.notes(),
      paypalPaymentId: this.paypalPaymentId,
      orderDetails: this.cart().items.map(item => ({
        productId: item.product.productId,
        quantity: item.quantity,
        unitPrice: item.product.price || 0
      } as CreateOrderDetailDto))
    };
    this.cartService.loading.next(true);
    this.orderService.create(order).subscribe({
      next: (res) => {
        this.cartService.clearCart();
        this.showCheckout.set(false);
        this.shippingAddress.set('');
        this.notes.set('');
        this.paypalPaymentId = null;
        this.toast.show('Order booked successfully!', 'success');
      },
      error: () => {
        this.toast.show('Failed to book order.', 'error');
        this.cartService.loading.next(false);
      }
    });
  }
}
