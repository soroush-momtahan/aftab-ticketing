import { Injectable, signal, computed, inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

export interface UserInfo {
  name: string;
  mobile: string;
  // هر فیلد دیگه‌ای که لازم داری
}

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private readonly platformId = inject(PLATFORM_ID);
  private readonly STORAGE_KEY = 'user_info';

  // Signal اصلی
  private readonly _user = signal<UserInfo | null>(null);

  // برای استفاده راحت در کامپوننت‌ها
  readonly user = this._user.asReadonly();
  readonly isLoggedIn = computed(() => !!this._user());
  readonly userName = computed(() => this._user()?.name ?? '');
  readonly userMobile = computed(() => this._user()?.mobile ?? '');

  constructor() {
    // فقط روی مرورگر از localStorage بخون
    if (isPlatformBrowser(this.platformId)) {
      this.loadFromStorage();
    }
  }

  /** بعد از لاگین یا ثبت‌نام موفق این متد رو صدا بزن */
  setUser(user: UserInfo): void {
    this._user.set(user);

    if (isPlatformBrowser(this.platformId)) {
      localStorage.setItem(this.STORAGE_KEY, JSON.stringify(user));
    }
  }

  /** خروج از حساب */
  clearUser(): void {
    this._user.set(null);

    if (isPlatformBrowser(this.platformId)) {
      localStorage.removeItem(this.STORAGE_KEY);
    }
  }

  private loadFromStorage(): void {
    try {
      const raw = localStorage.getItem(this.STORAGE_KEY);
      if (raw) {
        this._user.set(JSON.parse(raw));
      }
    } catch {
      this._user.set(null);
    }
  }
}
