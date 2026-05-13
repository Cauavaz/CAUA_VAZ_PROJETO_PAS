export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  name: string;
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  name: string;
  email: string;
  expiresAt: Date;
}

export interface UserDto {
  id: string;
  name: string;
  email: string;
  createdAt: Date;
}
