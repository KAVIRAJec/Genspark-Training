import { OrderDetailDto, CreateOrderDetailDto, UpdateOrderDetailDto } from './order-detail.model';

export interface OrderDto {
  orderId: number;
  userId: number;
  userName?: string;
  orderDate: string;
  totalAmount: number;
  status: string;
  shippingAddress?: string;
  notes?: string;
  isActive: boolean;
  paypalPaymentId?: string;
  orderDetails: OrderDetailDto[];
}
export interface CreateOrderDto {
  userId: number;
  shippingAddress?: string;
  notes?: string;
  paypalPaymentId?: string;
  orderDetails: CreateOrderDetailDto[];
}
export interface UpdateOrderDto {
  orderId: number;
  userId: number;
  shippingAddress?: string;
  notes?: string;
  status: string;
  orderDetails: UpdateOrderDetailDto[];
}
