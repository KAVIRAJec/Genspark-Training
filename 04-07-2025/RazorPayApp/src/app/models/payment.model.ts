// Payment result interface
export interface PaymentResult {
  success: boolean;
  message: string;
  paymentId?: string;
  timestamp?: Date;
}

// Form data interface
export interface PaymentFormData {
  amount: number;
  name: string;
  email: string;
  contact: string;
}
