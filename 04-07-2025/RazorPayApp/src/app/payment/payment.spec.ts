import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { of, throwError } from 'rxjs';

import { PaymentComponent } from './payment';
import { BackendService } from '../services/backend.service';
import { PaymentResult } from '../models/payment.model';
import { RazorpayResponse } from '../models/razorpay.model';

describe('PaymentComponent', () => {
  let component: PaymentComponent;
  let fixture: ComponentFixture<PaymentComponent>;
  let mockBackendService: jasmine.SpyObj<BackendService>;

  // Mock Razorpay
  const mockRazorpay = {
    open: jasmine.createSpy('open'),
    on: jasmine.createSpy('on'),
    close: jasmine.createSpy('close')
  };

  beforeEach(async () => {
    // Create spy objects for services
    mockBackendService = jasmine.createSpyObj('BackendService', [
      'isBackendAvailable',
      'getPaymentKey', 
      'createOrder',
      'verifyPayment'
    ]);

    // Mock window.Razorpay
    (window as any).Razorpay = jasmine.createSpy('Razorpay').and.returnValue(mockRazorpay);

    await TestBed.configureTestingModule({
      imports: [
        PaymentComponent,
        ReactiveFormsModule,
        HttpClientTestingModule
      ],
      providers: [
        { provide: BackendService, useValue: mockBackendService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(PaymentComponent);
    component = fixture.componentInstance;
  });

  describe('Component Initialization', () => {
    it('should create', () => {
      expect(component).toBeTruthy();
    });

    it('should initialize payment form with proper validators', () => {
      expect(component.paymentForm).toBeDefined();
      expect(component.paymentForm.get('amount')).toBeDefined();
      expect(component.paymentForm.get('name')).toBeDefined();
      expect(component.paymentForm.get('email')).toBeDefined();
      expect(component.paymentForm.get('contact')).toBeDefined();
    });

    it('should check backend availability on init', () => {
      mockBackendService.isBackendAvailable.and.returnValue(of(true));
      
      component.ngOnInit();
      
      expect(mockBackendService.isBackendAvailable).toHaveBeenCalled();
      expect(component.isBackendAvailable).toBe(true);
      expect(component.backendStatus).toBe('✅ Backend Connected - Ready for payments');
    });

    it('should handle backend unavailable on init', () => {
      mockBackendService.isBackendAvailable.and.returnValue(of(false));
      
      component.ngOnInit();
      
      expect(component.isBackendAvailable).toBe(false);
      expect(component.backendStatus).toBe('❌ Backend Offline - Please start the backend server');
    });

    it('should handle backend error on init', () => {
      mockBackendService.isBackendAvailable.and.returnValue(throwError('Network error'));
      
      component.ngOnInit();
      
      expect(component.isBackendAvailable).toBe(false);
      expect(component.backendStatus).toBe('❌ Backend Offline - Please start the backend server');
    });
  });

  describe('Form Validation', () => {
    beforeEach(() => {
      mockBackendService.isBackendAvailable.and.returnValue(of(true));
      fixture.detectChanges();
    });

    describe('Amount Field Validation', () => {
      it('should be invalid when amount is not provided', () => {
        const amountControl = component.paymentForm.get('amount');
        expect(amountControl?.valid).toBeFalsy();
        expect(amountControl?.errors?.['required']).toBeTruthy();
      });

      it('should be invalid when amount is less than 1', () => {
        const amountControl = component.paymentForm.get('amount');
        amountControl?.setValue(0);
        expect(amountControl?.valid).toBeFalsy();
        expect(amountControl?.errors?.['min']).toBeTruthy();
      });

      it('should be invalid when amount exceeds 100000', () => {
        const amountControl = component.paymentForm.get('amount');
        amountControl?.setValue(100001);
        expect(amountControl?.valid).toBeFalsy();
        expect(amountControl?.errors?.['max']).toBeTruthy();
      });

      it('should be valid when amount is within range', () => {
        const amountControl = component.paymentForm.get('amount');
        amountControl?.setValue(1000);
        expect(amountControl?.valid).toBeTruthy();
      });
    });

    describe('Name Field Validation', () => {
      it('should be invalid when name is not provided', () => {
        const nameControl = component.paymentForm.get('name');
        expect(nameControl?.valid).toBeFalsy();
        expect(nameControl?.errors?.['required']).toBeTruthy();
      });

      it('should be invalid when name is too short', () => {
        const nameControl = component.paymentForm.get('name');
        nameControl?.setValue('A');
        expect(nameControl?.valid).toBeFalsy();
        expect(nameControl?.errors?.['minlength']).toBeTruthy();
      });

      it('should be invalid when name is too long', () => {
        const nameControl = component.paymentForm.get('name');
        nameControl?.setValue('A'.repeat(51));
        expect(nameControl?.valid).toBeFalsy();
        expect(nameControl?.errors?.['maxlength']).toBeTruthy();
      });

      it('should be valid when name is proper length', () => {
        const nameControl = component.paymentForm.get('name');
        nameControl?.setValue('John Doe');
        expect(nameControl?.valid).toBeTruthy();
      });
    });

    describe('Email Field Validation', () => {
      it('should be invalid when email is not provided', () => {
        const emailControl = component.paymentForm.get('email');
        expect(emailControl?.valid).toBeFalsy();
        expect(emailControl?.errors?.['required']).toBeTruthy();
      });

      it('should be invalid when email format is incorrect', () => {
        const emailControl = component.paymentForm.get('email');
        emailControl?.setValue('invalid-email');
        expect(emailControl?.valid).toBeFalsy();
        expect(emailControl?.errors?.['email']).toBeTruthy();
      });

      it('should be valid when email format is correct', () => {
        const emailControl = component.paymentForm.get('email');
        emailControl?.setValue('test@example.com');
        expect(emailControl?.valid).toBeTruthy();
      });
    });

    describe('Contact Field Validation', () => {
      it('should be invalid when contact is not provided', () => {
        const contactControl = component.paymentForm.get('contact');
        expect(contactControl?.valid).toBeFalsy();
        expect(contactControl?.errors?.['required']).toBeTruthy();
      });

      it('should be invalid when contact format is incorrect', () => {
        const contactControl = component.paymentForm.get('contact');
        contactControl?.setValue('123456789'); // Wrong format
        expect(contactControl?.valid).toBeFalsy();
        expect(contactControl?.errors?.['pattern']).toBeTruthy();
      });

      it('should be valid when contact format is correct', () => {
        const contactControl = component.paymentForm.get('contact');
        contactControl?.setValue('9876543210');
        expect(contactControl?.valid).toBeTruthy();
      });
    });
  });

  describe('Form Submission', () => {
    beforeEach(() => {
      mockBackendService.isBackendAvailable.and.returnValue(of(true));
      fixture.detectChanges();
    });

    it('should not submit when form is invalid', () => {
      spyOn(component, 'initiatePayment' as any);
      
      component.onSubmit();
      
      expect(component['initiatePayment']).not.toHaveBeenCalled();
    });

    it('should mark form controls as touched when form is invalid', () => {
      component.onSubmit();
      
      expect(component.paymentForm.get('amount')?.touched).toBeTruthy();
      expect(component.paymentForm.get('name')?.touched).toBeTruthy();
      expect(component.paymentForm.get('email')?.touched).toBeTruthy();
      expect(component.paymentForm.get('contact')?.touched).toBeTruthy();
    });

    it('should not submit when backend is unavailable', () => {
      component.isBackendAvailable = false;
      spyOn(component, 'initiatePayment' as any);
      
      // Fill valid form
      component.paymentForm.patchValue({
        amount: 1000,
        name: 'John Doe',
        email: 'john@example.com',
        contact: '9876543210'
      });
      
      component.onSubmit();
      
      expect(component['initiatePayment']).not.toHaveBeenCalled();
      expect(component.paymentResult?.success).toBeFalsy();
      expect(component.paymentResult?.message).toContain('Backend server is not available');
    });

    it('should initiate payment when form is valid and backend is available', () => {
      spyOn(component, 'initiatePayment' as any);
      
      // Fill valid form
      component.paymentForm.patchValue({
        amount: 1000,
        name: 'John Doe',
        email: 'john@example.com',
        contact: '9876543210'
      });
      
      component.onSubmit();
      
      expect(component['initiatePayment']).toHaveBeenCalled();
    });
  });

  describe('Payment Processing', () => {
    beforeEach(() => {
      mockBackendService.isBackendAvailable.and.returnValue(of(true));
      fixture.detectChanges();
      
      // Setup valid form
      component.paymentForm.patchValue({
        amount: 1000,
        name: 'John Doe',
        email: 'john@example.com',
        contact: '9876543210'
      });
    });
  });

  describe('Payment Error Handling', () => {
    it('should handle payment dismissal', () => {
      component['handlePaymentDismiss']();
      
      expect(component.isProcessing).toBeFalsy();
      expect(component.paymentResult?.success).toBeFalsy();
      expect(component.paymentResult?.message).toContain('Payment was cancelled or dismissed');
    });

    it('should handle payment error', () => {
      const errorMessage = 'Test error message';
      
      component['handlePaymentError'](errorMessage);
      
      expect(component.isProcessing).toBeFalsy();
      expect(component.paymentResult?.success).toBeFalsy();
      expect(component.paymentResult?.message).toBe(errorMessage);
    });
  });

  describe('Utility Methods', () => {
    beforeEach(() => {
      mockBackendService.isBackendAvailable.and.returnValue(of(true));
      fixture.detectChanges();
    });

    it('should reset form', () => {
      // Set some values first
      component.paymentForm.patchValue({
        amount: 1000,
        name: 'John Doe',
        email: 'john@example.com',
        contact: '9876543210'
      });
      component.paymentResult = {
        success: true,
        message: 'Test',
        paymentId: 'pay_123',
        timestamp: new Date()
      };
      component.isProcessing = true;
      
      component.resetForm();
      
      expect(component.paymentForm.value.amount).toBeNull();
      expect(component.paymentForm.value.name).toBeNull();
      expect(component.paymentForm.value.email).toBeNull();
      expect(component.paymentForm.value.contact).toBeNull();
      expect(component.paymentResult).toBeNull();
      expect(component.isProcessing).toBeFalsy();
    });

    it('should clear result', () => {
      component.paymentResult = {
        success: true,
        message: 'Test',
        paymentId: 'pay_123',
        timestamp: new Date()
      };
      
      component.clearResult();
      
      expect(component.paymentResult).toBeNull();
    });
  });

  describe('Component Rendering', () => {
    beforeEach(() => {
      mockBackendService.isBackendAvailable.and.returnValue(of(true));
      fixture.detectChanges();
    });

    it('should render payment form', () => {
      const compiled = fixture.nativeElement;
      expect(compiled.querySelector('form')).toBeTruthy();
      expect(compiled.querySelector('input[formControlName="amount"]')).toBeTruthy();
      expect(compiled.querySelector('input[formControlName="name"]')).toBeTruthy();
      expect(compiled.querySelector('input[formControlName="email"]')).toBeTruthy();
      expect(compiled.querySelector('input[formControlName="contact"]')).toBeTruthy();
    });

    it('should render submit button', () => {
      const compiled = fixture.nativeElement;
      const submitButton = compiled.querySelector('button[type="submit"]');
      expect(submitButton).toBeTruthy();
      expect(submitButton.textContent).toContain('Pay with Razorpay');
    });

    it('should disable submit button when form is invalid', () => {
      const compiled = fixture.nativeElement;
      const submitButton = compiled.querySelector('button[type="submit"]');
      expect(submitButton.disabled).toBeTruthy();
    });

    it('should enable submit button when form is valid and backend is available', () => {
      component.paymentForm.patchValue({
        amount: 1000,
        name: 'John Doe',
        email: 'john@example.com',
        contact: '9876543210'
      });
      fixture.detectChanges();
      
      const compiled = fixture.nativeElement;
      const submitButton = compiled.querySelector('button[type="submit"]');
      expect(submitButton.disabled).toBeFalsy();
    });

    it('should show backend status', () => {
      component.backendStatus = 'Test status';
      fixture.detectChanges();
      
      const compiled = fixture.nativeElement;
      expect(compiled.textContent).toContain('Test status');
    });

    it('should show payment result when available', () => {
      component.paymentResult = {
        success: true,
        message: 'Payment successful',
        paymentId: 'pay_123',
        timestamp: new Date()
      };
      fixture.detectChanges();
      
      const compiled = fixture.nativeElement;
      expect(compiled.textContent).toContain('Payment successful');
      expect(compiled.textContent).toContain('pay_123');
    });
  });
});
