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
