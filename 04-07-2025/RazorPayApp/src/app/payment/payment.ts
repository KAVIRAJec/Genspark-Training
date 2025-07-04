import { Component, OnInit, ChangeDetectorRef, NgZone } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { BackendService } from '../services/backend.service';
import { PaymentResult, PaymentFormData } from '../models/payment.model';
import { RazorpayOptions, RazorpayResponse } from '../models/razorpay.model';

declare global {
  interface Window {
    Razorpay: any;
  }
}

@Component({
  selector: 'app-payment',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, HttpClientModule],
  templateUrl: './payment.html',
  styleUrls: ['./payment.css']
})
export class PaymentComponent implements OnInit {
  paymentForm: FormGroup;
  paymentResult: PaymentResult | null = null;
  isProcessing = false;
  isBackendAvailable = false;
  backendStatus = 'Checking backend...';

  constructor(
    private fb: FormBuilder,
    private backendService: BackendService,
    private cdr: ChangeDetectorRef,
    private zone: NgZone
  ) {
    this.paymentForm = this.fb.group({
      amount: [null, [Validators.required, Validators.min(1), Validators.max(100000)]],
      name: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(50)]],
      email: ['', [Validators.required, Validators.email]],
      contact: ['', [Validators.required, Validators.pattern(/^[6-9]\d{9}$/)]] // Valid Indian mobile number
    });
  }

  ngOnInit(): void {
    this.checkBackendAvailability();
  }

  private checkBackendAvailability(): void {
    this.backendService.isBackendAvailable().subscribe({
      next: (available) => {
        this.isBackendAvailable = available;
        this.backendStatus = available ? 
          '✅ Backend Connected - Ready for payments' : 
          '❌ Backend Offline - Please start the backend server';
      },
      error: () => {
        this.isBackendAvailable = false;
        this.backendStatus = '❌ Backend Offline - Please start the backend server';
      }
    });
  }

  // Getter methods for form validation
  get amount() { return this.paymentForm.get('amount'); }
  get name() { return this.paymentForm.get('name'); }
  get email() { return this.paymentForm.get('email'); }
  get contact() { return this.paymentForm.get('contact'); }

  onSubmit(): void {
    if (this.paymentForm.invalid) {
      this.markFormGroupTouched();
      return;
    }

    if (!this.isBackendAvailable) {
      this.handlePaymentError('Backend server is not available. Please start the backend server and try again.');
      return;
    }

    this.initiatePayment();
  }

  private markFormGroupTouched(): void {
    Object.keys(this.paymentForm.controls).forEach(key => {
      const control = this.paymentForm.get(key);
      control?.markAsTouched();
    });
  }

  private initiatePayment(): void {
    this.isProcessing = true;
    this.paymentResult = null;

    const formData: PaymentFormData = {
      amount: this.paymentForm.value.amount,
      name: this.paymentForm.value.name,
      email: this.paymentForm.value.email,
      contact: this.paymentForm.value.contact
    };

    this.processRazorpayPayment(formData);
  }

  private async processRazorpayPayment(formData: PaymentFormData): Promise<void> {
    try {
      // Get Razorpay key from backend
      const keyResponse = await this.backendService.getPaymentKey().toPromise();
      if (!keyResponse?.success || !keyResponse.key) {
        throw new Error('Failed to get payment key from backend');
      }

      // Create order via backend
      const orderResponse = await this.backendService.createOrder(formData.amount).toPromise();
      if (!orderResponse?.success || !orderResponse.order) {
        throw new Error('Failed to create order');
      }

      const order = orderResponse.order;

      // Check if Razorpay is loaded
      if (!window.Razorpay) {
        throw new Error('Razorpay script not loaded. Please refresh the page.');
      }

      console.log('Creating Razorpay payment with order:', order);

      // Configure Razorpay options using simplified objects
      const options: RazorpayOptions = {
        key: keyResponse.key,
        amount: order.amount,
        currency: order.currency,
        name: 'UPI Payment App',
        description: 'UPI Payment via Razorpay',
        order_id: order.id,
        image: 'https://razorpay.com/assets/razorpay-glyph.svg',
        prefill: {
          name: formData.name,
          email: formData.email,
          contact: formData.contact
        },
        theme: {
          color: '#528FF0'
        },
        modal: {
          ondismiss: () => this.handlePaymentDismiss(),
          escape: true,
          backdropclose: true
        },
        handler: (response: RazorpayResponse) => this.handlePaymentSuccess(response)
      };

      // Minimal configuration that should show UPI
      delete (options as any).method;
      delete (options as any).config;
      
      // Only set essential options
      (options as any).send_sms_hash = true;
      (options as any).remember_customer = false;

      console.log('Razorpay options with minimal config:', options);

      // Add error handler
      (options as any).error = (error: any) => {
        this.zone.run(() => {
          console.log('Razorpay error handler called:', error);
          this.handlePaymentError('Payment failed: ' + (error.description || error.reason || 'Unknown error'));
        });
      };

      // Create and open Razorpay checkout
      const rzp = new window.Razorpay(options);
      
      // Add event listeners for debugging
      rzp.on('payment.failed', (response: any) => {
        this.zone.run(() => {
          console.log('Payment failed response:', response);
          this.handlePaymentError(`Payment failed: ${response.error.description || response.error.reason || 'Unknown error'}`);
        });
      });

      rzp.on('payment.authorize', (response: any) => {
        this.zone.run(() => {
          console.log('Payment authorized:', response);
        });
      });

      rzp.on('payment.cancel', () => {
        this.zone.run(() => {
          console.log('Payment cancelled by user');
          this.handlePaymentDismiss();
        });
      });
      
      console.log('Opening Razorpay checkout...');
      rzp.open();

    } catch (error) {
      console.error('Payment initiation error:', error);
      this.handlePaymentError('Failed to initiate payment: ' + (error as Error).message);
    }
  }

  private handlePaymentSuccess(response: RazorpayResponse): void {
    console.log('Payment success callback received:', response);
    
    // Run inside Angular zone to ensure change detection
    this.zone.run(() => {
      // Verify payment with backend
      this.backendService.verifyPayment({
        razorpay_order_id: response.razorpay_order_id!,
        razorpay_payment_id: response.razorpay_payment_id,
        razorpay_signature: response.razorpay_signature!
      }).subscribe({
        next: (verifyResponse) => {
          console.log('Verification response:', verifyResponse);
          
          if (verifyResponse?.success) {
            this.isProcessing = false;
            this.paymentResult = {
              success: true,
              message: 'Payment completed and verified successfully!',
              paymentId: response.razorpay_payment_id,
              timestamp: new Date()
            };
            
            console.log('Current UI state - isProcessing:', this.isProcessing);
            console.log('Current UI state - paymentResult:', this.paymentResult);
            
            // Force change detection
            this.cdr.detectChanges();
            console.log('Change detection triggered after success');
            
            // Don't auto-reset the form so user can see the result
            // setTimeout(() => {
            //   this.resetForm();
            // }, 3000);
          } else {
            this.handlePaymentError('Payment verification failed');
          }
        },
        error: (error) => {
          console.error('Payment verification error:', error);
          this.handlePaymentError('Payment verification failed: ' + error.message);
        }
      });
    });
  }

  private handlePaymentDismiss(): void {
    this.zone.run(() => {
      this.isProcessing = false;
      this.paymentResult = {
        success: false,
        message: 'Payment was cancelled or dismissed',
        timestamp: new Date()
      };
      console.log('Payment dismissed, setting result:', this.paymentResult);
      this.cdr.detectChanges();
    });
  }

  private handlePaymentError(errorMessage: string): void {
    this.zone.run(() => {
      this.isProcessing = false;
      this.paymentResult = {
        success: false,
        message: errorMessage,
        timestamp: new Date()
      };
      console.log('Payment error, setting result:', this.paymentResult);
      this.cdr.detectChanges();
    });
  }

  resetForm(): void {
    this.zone.run(() => {
      this.paymentForm.reset();
      this.paymentResult = null;
      this.isProcessing = false;
      console.log('Form reset, current state:', { isProcessing: this.isProcessing, paymentResult: this.paymentResult });
      this.cdr.detectChanges();
    });
  }

  clearResult(): void {
    this.zone.run(() => {
      this.paymentResult = null;
      console.log('Result cleared, current state:', { paymentResult: this.paymentResult });
      this.cdr.detectChanges();
    });
  }
}
