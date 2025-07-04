import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { 
  BackendService,
  BackendOrder,
  BackendResponse,
  CreateOrderResponse,
  KeyResponse,
  VerifyResponse 
} from './backend.service';

describe('BackendService', () => {
  let service: BackendService;
  let httpMock: HttpTestingController;
  const baseUrl = 'http://localhost:3000/api';

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [BackendService]
    });
    service = TestBed.inject(BackendService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('isBackendAvailable', () => {
    it('should return true when backend is available', (done) => {
      const mockResponse = { status: 'healthy' };

      service.isBackendAvailable().subscribe(result => {
        expect(result).toBe(true);
        done();
      });

      const req = httpMock.expectOne(`${baseUrl}/health`);
      expect(req.request.method).toBe('GET');
      req.flush(mockResponse);
    });

    it('should return false when backend is not available', (done) => {
      service.isBackendAvailable().subscribe(result => {
        expect(result).toBe(false);
        done();
      });

      const req = httpMock.expectOne(`${baseUrl}/health`);
      expect(req.request.method).toBe('GET');
      req.error(new ErrorEvent('Network error'));
    });

    it('should return false on HTTP error status', (done) => {
      service.isBackendAvailable().subscribe(result => {
        expect(result).toBe(false);
        done();
      });

      const req = httpMock.expectOne(`${baseUrl}/health`);
      expect(req.request.method).toBe('GET');
      req.flush('Server Error', { status: 500, statusText: 'Internal Server Error' });
    });
  });

  describe('getPaymentKey', () => {
    it('should return payment key successfully', () => {
      const mockResponse: KeyResponse = {
        success: true,
        message: 'Key retrieved successfully',
        key: 'rzp_test_key123'
      };

      service.getPaymentKey().subscribe(response => {
        expect(response.success).toBe(true);
        expect(response.key).toBe('rzp_test_key123');
        expect(response.message).toBe('Key retrieved successfully');
      });

      const req = httpMock.expectOne(`${baseUrl}/payment/key`);
      expect(req.request.method).toBe('GET');
      req.flush(mockResponse);
    });

    it('should handle payment key fetch error', () => {
      const mockErrorResponse: KeyResponse = {
        success: false,
        error: 'Key not found'
      };

      service.getPaymentKey().subscribe(response => {
        expect(response.success).toBe(false);
        expect(response.error).toBe('Key not found');
        expect(response.key).toBeUndefined();
      });

      const req = httpMock.expectOne(`${baseUrl}/payment/key`);
      expect(req.request.method).toBe('GET');
      req.flush(mockErrorResponse);
    });

    it('should handle HTTP error responses', () => {
      service.getPaymentKey().subscribe({
        next: () => fail('Should have failed'),
        error: (error) => {
          expect(error).toBeDefined();
          expect(error.message).toContain('Error Code: 500');
        }
      });

      const req = httpMock.expectOne(`${baseUrl}/payment/key`);
      req.flush('Server Error', { status: 500, statusText: 'Internal Server Error' });
    });
  });

  describe('createOrder', () => {
    it('should create order successfully with default currency', () => {
      const amount = 1000;
      const mockOrder: BackendOrder = {
        id: 'order_123',
        amount: 100000,
        currency: 'INR',
        receipt: 'receipt_123',
        status: 'created'
      };
      
      const mockResponse: CreateOrderResponse = {
        success: true,
        message: 'Order created successfully',
        order: mockOrder
      };

      service.createOrder(amount).subscribe(response => {
        expect(response.success).toBe(true);
        expect(response.order).toEqual(mockOrder);
        expect(response.message).toBe('Order created successfully');
      });

      const req = httpMock.expectOne(`${baseUrl}/payment/create-order`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body.amount).toBe(amount);
      expect(req.request.body.currency).toBe('INR');
      expect(req.request.body.receipt).toContain('receipt_');
      req.flush(mockResponse);
    });

    it('should create order with custom currency', () => {
      const amount = 1000;
      const currency = 'USD';
      const mockOrder: BackendOrder = {
        id: 'order_123',
        amount: 100000,
        currency: 'USD',
        receipt: 'receipt_123',
        status: 'created'
      };
      
      const mockResponse: CreateOrderResponse = {
        success: true,
        message: 'Order created successfully',
        order: mockOrder
      };

      service.createOrder(amount, currency).subscribe(response => {
        expect(response.success).toBe(true);
        expect(response.order?.currency).toBe('USD');
      });

      const req = httpMock.expectOne(`${baseUrl}/payment/create-order`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body.currency).toBe('USD');
      req.flush(mockResponse);
    });

    it('should handle order creation error', () => {
      const amount = 1000;
      const mockErrorResponse: CreateOrderResponse = {
        success: false,
        error: 'Order creation failed'
      };

      service.createOrder(amount).subscribe(response => {
        expect(response.success).toBe(false);
        expect(response.error).toBe('Order creation failed');
        expect(response.order).toBeUndefined();
      });

      const req = httpMock.expectOne(`${baseUrl}/payment/create-order`);
      expect(req.request.method).toBe('POST');
      req.flush(mockErrorResponse);
    });

    it('should handle network errors', () => {
      const amount = 1000;

      service.createOrder(amount).subscribe({
        next: () => fail('Should have failed'),
        error: (error) => {
          expect(error).toBeDefined();
          expect(error.message).toBeDefined();
        }
      });

      const req = httpMock.expectOne(`${baseUrl}/payment/create-order`);
      req.error(new ErrorEvent('Network error'));
    });
  });

  describe('verifyPayment', () => {
    it('should verify payment successfully', () => {
      const paymentData = {
        razorpay_order_id: 'order_123',
        razorpay_payment_id: 'pay_123',
        razorpay_signature: 'signature_123'
      };

      const mockPaymentDetails = {
        order_id: 'order_123',
        payment_id: 'pay_123',
        verified: true,
        timestamp: '2025-07-04T10:00:00Z'
      };

      const mockResponse: VerifyResponse = {
        success: true, 
        message: 'Payment verified successfully',
        payment: mockPaymentDetails
      };

      service.verifyPayment(paymentData).subscribe(response => {
        expect(response.success).toBe(true);
        expect(response.message).toBe('Payment verified successfully');
        expect(response.payment).toEqual(mockPaymentDetails);
        expect(response.payment?.verified).toBe(true);
      });

      const req = httpMock.expectOne(`${baseUrl}/payment/verify`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(paymentData);
      req.flush(mockResponse);
    });

    it('should handle payment verification failure', () => {
      const paymentData = {
        razorpay_order_id: 'order_123',
        razorpay_payment_id: 'pay_123',
        razorpay_signature: 'invalid_signature'
      };

      const mockResponse: VerifyResponse = {
        success: false, 
        error: 'Invalid signature'
      };

      service.verifyPayment(paymentData).subscribe(response => {
        expect(response.success).toBe(false);
        expect(response.error).toBe('Invalid signature');
        expect(response.payment).toBeUndefined();
      });

      const req = httpMock.expectOne(`${baseUrl}/payment/verify`);
      expect(req.request.method).toBe('POST');
      req.flush(mockResponse);
    });

    it('should send correct payload structure', () => {
      const paymentData = {
        razorpay_order_id: 'order_123',
        razorpay_payment_id: 'pay_123',
        razorpay_signature: 'signature_123'
      };

      service.verifyPayment(paymentData).subscribe();

      const req = httpMock.expectOne(`${baseUrl}/payment/verify`);
      expect(req.request.body).toEqual(paymentData);
      expect(req.request.body.razorpay_order_id).toBe('order_123');
      expect(req.request.body.razorpay_payment_id).toBe('pay_123');
      expect(req.request.body.razorpay_signature).toBe('signature_123');
      
      req.flush({
        success: true,
        message: 'Verified'
      });
    });

    it('should handle verification timeout errors', () => {
      const paymentData = {
        razorpay_order_id: 'order_123',
        razorpay_payment_id: 'pay_123',
        razorpay_signature: 'signature_123'
      };

      service.verifyPayment(paymentData).subscribe({
        next: () => fail('Should have failed'),
        error: (error) => {
          expect(error).toBeDefined();
        }
      });

      const req = httpMock.expectOne(`${baseUrl}/payment/verify`);
      req.error(new ErrorEvent('timeout'));
    });
  });

  describe('getPaymentDetails', () => {
    it('should get payment details successfully', () => {
      const paymentId = 'pay_123';
      const mockPaymentDetails = {
        id: 'pay_123',
        amount: 100000,
        currency: 'INR',
        status: 'captured',
        order_id: 'order_123'
      };

      service.getPaymentDetails(paymentId).subscribe(response => {
        expect(response).toEqual(mockPaymentDetails);
        expect(response.id).toBe(paymentId);
        expect(response.status).toBe('captured');
      });

      const req = httpMock.expectOne(`${baseUrl}/payment/${paymentId}`);
      expect(req.request.method).toBe('GET');
      req.flush(mockPaymentDetails);
    });

    it('should handle payment details fetch error', () => {
      const paymentId = 'invalid_pay_id';

      service.getPaymentDetails(paymentId).subscribe({
        next: () => fail('Should have failed'),
        error: (error) => {
          expect(error).toBeDefined();
          expect(error.message).toContain('Error Code: 404');
        }
      });

      const req = httpMock.expectOne(`${baseUrl}/payment/${paymentId}`);
      expect(req.request.method).toBe('GET');
      req.flush('Payment not found', { status: 404, statusText: 'Not Found' });
    });

    it('should use correct URL with payment ID', () => {
      const testPaymentIds = ['pay_123', 'pay_ABC456', 'payment_789'];

      testPaymentIds.forEach(paymentId => {
        service.getPaymentDetails(paymentId).subscribe();

        const req = httpMock.expectOne(`${baseUrl}/payment/${paymentId}`);
        expect(req.request.url).toBe(`${baseUrl}/payment/${paymentId}`);
        req.flush({ id: paymentId });
      });
    });
  });

  describe('Error Handling', () => {
    it('should handle client-side errors', () => {
      const clientError = new ErrorEvent('Client Error', {
        message: 'Network connection failed'
      });

      service.getPaymentKey().subscribe({
        next: () => fail('Should have failed'),
        error: (error) => {
          expect(error.message).toContain('Network connection failed');
        }
      });

      const req = httpMock.expectOne(`${baseUrl}/payment/key`);
      req.error(clientError);
    });

    it('should handle server-side errors with custom message', () => {
      const serverErrorResponse = {
        message: 'Custom server error message'
      };

      service.getPaymentKey().subscribe({
        next: () => fail('Should have failed'),
        error: (error) => {
          expect(error.message).toContain('Custom server error message');
        }
      });

      const req = httpMock.expectOne(`${baseUrl}/payment/key`);
      req.flush(serverErrorResponse, { status: 500, statusText: 'Internal Server Error' });
    });

    it('should handle server-side errors without custom message', () => {
      service.getPaymentKey().subscribe({
        next: () => fail('Should have failed'),
        error: (error) => {
          expect(error.message).toContain('Error Code: 500');
          expect(error.message).toContain('Internal Server Error');
        }
      });

      const req = httpMock.expectOne(`${baseUrl}/payment/key`);
      req.flush('Generic error', { status: 500, statusText: 'Internal Server Error' });
    });

    it('should log errors to console', () => {
      spyOn(console, 'error');

      service.getPaymentKey().subscribe({
        next: () => fail('Should have failed'),
        error: () => {
          expect(console.error).toHaveBeenCalledWith('Backend API Error:', jasmine.any(String));
        }
      });

      const req = httpMock.expectOne(`${baseUrl}/payment/key`);
      req.flush('Error', { status: 500, statusText: 'Internal Server Error' });
    });
  });

  describe('Model Interfaces', () => {
    it('should create BackendOrder object correctly', () => {
      const order: BackendOrder = {
        id: 'order_123',
        amount: 100000,
        currency: 'INR',
        receipt: 'receipt_123',
        status: 'created'
      };
      
      expect(order.id).toBe('order_123');
      expect(order.amount).toBe(100000);
      expect(order.currency).toBe('INR');
      expect(order.receipt).toBe('receipt_123');
      expect(order.status).toBe('created');
    });

    it('should create BackendResponse object correctly', () => {
      const response: BackendResponse = {
        success: true,
        message: 'Success message',
        error: 'Error message'
      };
      
      expect(response.success).toBe(true);
      expect(response.message).toBe('Success message');
      expect(response.error).toBe('Error message');
    });

    it('should create CreateOrderResponse object correctly', () => {
      const order: BackendOrder = {
        id: 'order_123',
        amount: 100000,
        currency: 'INR',
        receipt: 'receipt_123',
        status: 'created'
      };
      
      const response: CreateOrderResponse = {
        success: true,
        message: 'Order created',
        order: order
      };
      
      expect(response.success).toBe(true);
      expect(response.message).toBe('Order created');
      expect(response.order).toBe(order);
    });

    it('should create KeyResponse object correctly', () => {
      const response: KeyResponse = {
        success: true,
        message: 'Key retrieved',
        key: 'rzp_test_key'
      };
      
      expect(response.success).toBe(true);
      expect(response.message).toBe('Key retrieved');
      expect(response.key).toBe('rzp_test_key');
    });

    it('should create VerifyResponse object correctly', () => {
      const paymentData = {
        order_id: 'order_123',
        payment_id: 'pay_123',
        verified: true,
        timestamp: '2025-07-04T10:00:00Z'
      };
      
      const response: VerifyResponse = {
        success: true,
        message: 'Verified',
        payment: paymentData
      };
      
      expect(response.success).toBe(true);
      expect(response.message).toBe('Verified');
      expect(response.payment).toBe(paymentData);
    });
  });
});
