// AUTH MODELS
export interface LoginDto {
  email: string;
  password: string;
}

export interface RegisterDto {
  userName: string;
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  phone?: string;
  address?: string;
}

export interface AuthenticationResult {
  success: boolean;
  message: string;
  token: string;
  refreshToken: string;
  tokenExpiration: string;
  user?: UserDto;
}

export interface RefreshTokenDto {
  refreshToken: string;
}

// CART MODELS
export interface CartItem {
  product: ProductDto;
  quantity: number;
}
export interface Cart {
  items: CartItem[];
}

// CATEGORY MODELS
export interface CategoryDto {
  categoryId: number;
  name: string;
  isActive: boolean;
  productCount: number;
}
export interface CreateCategoryDto {
  name: string;
}
export interface UpdateCategoryDto {
  categoryId: number;
  name: string;
}

// COLOR MODELS
export interface ColorDto {
  colorId: number;
  name: string;
  hexCode: string;
  isActive: boolean;
  productCount: number;
}
export interface CreateColorDto {
  name: string;
  hexCode: string;
}
export interface UpdateColorDto {
  colorId: number;
  name: string;
  hexCode: string;
}

// CONTACT-US MODELS
export interface ContactUsDto {
  contactId: number;
  name: string;
  email: string;
  subject?: string;
  message: string;
  createdDate: string;
  isRead: boolean;
  isActive: boolean;
}
export interface CreateContactUsDto {
  name: string;
  email: string;
  subject?: string;
  message: string;
}
export interface UpdateContactUsDto {
  contactId: number;
  name: string;
  email: string;
  subject?: string;
  message: string;
}

// MODEL MODELS
export interface ModelDto {
  modelId: number;
  name: string;
  createdDate: string;
  isActive: boolean;
  productCount: number;
}
export interface CreateModelDto {
  name: string;
}
export interface UpdateModelDto {
  modelId: number;
  name: string;
}

// NEWS MODELS
export interface NewsDto {
  newsId: number;
  title: string;
  content: string;
  summary?: string;
  image?: string;
  createdDate: string;
  updatedDate?: string;
  isPublished: boolean;
  isActive: boolean;
  authorId: number;
  authorName: string;
}
export interface CreateNewsDto {
  title: string;
  content: string;
  summary?: string;
  image?: string;
  isPublished?: boolean;
  authorId: number;
}
export interface UpdateNewsDto {
  newsId: number;
  title: string;
  content: string;
  summary?: string;
  image?: string;
  isPublished?: boolean;
  authorId: number;
}

// ORDER & ORDER DETAIL MODELS
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
  orderDetails: OrderDetailDto[];
}
export interface CreateOrderDto {
  userId: number;
  shippingAddress?: string;
  notes?: string;
  orderDetails: CreateOrderDetailDto[];
}
export interface CreateOrderDetailDto {
  productId: number;
  quantity: number;
  unitPrice: number;
}
export interface UpdateOrderDto {
  orderId: number;
  userId: number;
  shippingAddress?: string;
  notes?: string;
  status: string;
  orderDetails: UpdateOrderDetailDto[];
}
export interface UpdateOrderDetailDto {
  orderDetailId: number;
  productId: number;
  quantity: number;
  unitPrice: number;
}

// PRODUCT MODELS
export interface ProductDto {
  productId: number;
  productName: string;
  image?: string;
  price?: number;
  userId?: number;
  userName?: string;
  categoryId?: number;
  categoryName?: string;
  colorId?: number;
  colorName?: string;
  colorHexCode?: string;
  modelId?: number;
  modelName?: string;
  storageId?: number;
  sellStartDate?: string;
  sellEndDate?: string;
  isNew?: number;
  isActive: boolean;
}
export interface CreateProductDto {
  productName: string;
  image?: string;
  price?: number;
  userId?: number;
  categoryId?: number;
  colorId?: number;
  modelId?: number;
  storageId?: number;
  sellStartDate?: string;
  sellEndDate?: string;
  isNew?: number;
  isActive?: boolean;
}
export interface UpdateProductDto {
  productId: number;
  productName: string;
  image?: string;
  price?: number;
  userId?: number;
  categoryId?: number;
  colorId?: number;
  modelId?: number;
  storageId?: number;
  sellStartDate?: string;
  sellEndDate?: string;
  isNew?: number;
  isActive?: boolean;
}

// USER MODELS
export interface UserDto {
  userId: number;
  userName: string;
  email: string;
  firstName?: string;
  lastName?: string;
  phone?: string;
  address?: string;
  createdDate: string;
  isActive: boolean;
  role: string;
  orderCount: number;
  productCount: number;
  newsCount: number;
}
export interface CreateUserDto {
  userName: string;
  email: string;
  password: string;
  firstName?: string;
  lastName?: string;
  phone?: string;
  address?: string;
}
export interface UpdateUserDto {
  userId: number;
  userName: string;
  email: string;
  firstName?: string;
  lastName?: string;
  phone?: string;
  address?: string;
  isActive?: boolean;
  role?: string;
}
