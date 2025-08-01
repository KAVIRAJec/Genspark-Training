import { UserDto } from "./user.model";

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
