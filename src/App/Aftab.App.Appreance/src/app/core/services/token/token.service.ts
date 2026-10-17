import {Injectable, inject, PLATFORM_ID, REQUEST, signal } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

@Injectable({ providedIn: 'root' })
export class TokenService {
  private readonly platformId = inject(PLATFORM_ID);

  // خواندن آبجکت Request وب در زمان اجرای SSR
  private readonly req = inject(REQUEST, { optional: true });

  public readonly hasToken = signal(false);

  getToken(): string | null {
    let cookieString = '';

    if (isPlatformBrowser(this.platformId)) {
      // ۱. اگر در مرورگر هستیم
      cookieString = document.cookie;
    } else if (this.req) {
      // ۲. اگر در سرور (SSR) هستیم
      cookieString = this.req.headers.get('cookie') || '';
    }

    // استخراج توکن از کوکی با یک Regex ساده و سریع
    const match = cookieString.match(/(^| )auth_token=([^;]+)/);
    return match ? match[2] : null;
  }

  setToken(token: string): void {
    if (isPlatformBrowser(this.platformId)) {
      // ذخیره توکن به صورت امن (فقط در مرورگر معنی دارد)
      document.cookie = `auth_token=${token}; path=/; max-age=7200; SameSite=Strict; Secure`;
      this.hasToken.set(true);
    }
  }

  removeToken(): void {
    if (isPlatformBrowser(this.platformId)) {
      document.cookie = 'auth_token=; path=/; expires=Thu, 01 Jan 1970 00:00:00 GMT';
      this.hasToken.set(false);
    }
  }
}
