export interface OrderDetailDto {
  orderDetailId: number;
  orderId: number;
  productId: number;
  productName?: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
  isActive: boolean;
}
export interface CreateOrderDetailDto {
  productId: number;
  quantity: number;
  unitPrice: number;
}
export interface UpdateOrderDetailDto {
  orderDetailId: number;
  productId: number;
  quantity: number;
  unitPrice: number;
}
