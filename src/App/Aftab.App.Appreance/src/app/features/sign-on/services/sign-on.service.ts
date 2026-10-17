import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

// --- Interfaces ---
export interface SendOtpForNewUserRequest {
  mobile: string;
}
export interface SendOtpForNewUserResponse {
  isExistingUser: boolean;
}

export interface SignInWithPasswordRequest {
  mobile: string;
  password: string;
}
export interface SignInWithOtpRequest {
  mobile: string;
  otp: string;
}
export interface SignInResponse {
  token: string;
}

export interface SignUpRequest {
  tempToken: string;
  password: string;
  confirmPassword: string;
  firstName: string;
  lastName: string;
}
export interface SignUpResponse {
  token: string;
}

export interface VerifyMobileResponse {
  isNewUser: boolean;
  tempToken: string;
}

@Injectable({ providedIn: 'root' })
export class SignOnService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api'; // بسته به تنظیمات پروکسی

  sendOtpForNewUser(data: SendOtpForNewUserRequest): Observable<SendOtpForNewUserResponse> {
    return this.http.post<SendOtpForNewUserResponse>(`${this.baseUrl}/newUser/sendOtp`, data);
  }

  sendOtpForExistUser(mobile: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/otp/${mobile}`, {});
  }

  // در بک‌اند C# شما، موبایل و کد در URL هستند و Body نداریم
  verifyMobile(mobile: string, otpCode: string): Observable<VerifyMobileResponse> {
    return this.http.post<VerifyMobileResponse>(
      `${this.baseUrl}/mobile/${mobile}/verify/${otpCode}`,
      {},
    );
  }

  signInWithPassword(data: SignInWithPasswordRequest): Observable<SignInResponse> {
    return this.http.post<SignInResponse>(`${this.baseUrl}/signIn`, data);
  }

  signUp(data: SignUpRequest): Observable<SignUpResponse> {
    return this.http.post<SignUpResponse>(`${this.baseUrl}/signUp`, data);
  }
}
