// Login credentials sent to the API
export interface LoginCredentials {
  email: string;
  password: string;
}

// Registration data sent to the API
export interface RegistrationCredentials {
  email: string;
  fullName: string;
  password: string;
  userName: string;
}

// Response from login/register API
export interface AuthResponse {
  id: number;
  userName: string;
  fullName: string;
  email: string;
  token: string;
}