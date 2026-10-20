// import { HttpClient } from '@angular/common/http';
// import { Injectable, Signal,  computed, inject } from '@angular/core';
// import { catchError, shareReplay, Observable, defer } from 'rxjs';
// import { of } from 'rxjs';
// import { toSignal } from '@angular/core/rxjs-interop';
//
// const ANONYMOUS: Session = null;
// const CACHE_SIZE = 1;
//
// @Injectable({
//   providedIn: 'root'
// })
// export class AuthService {
//   private readonly http = inject(HttpClient);
//   private session$: Observable<Session> | null = null;
//
//   // Create a signal from the getSession() observable.
//   // Maybe in the future this can be a Resource once the api stabilizes.
//   public session: Signal<Session> = toSignal(
//     defer(() => this.getSession()), // Defer the getSession call
//     { initialValue: ANONYMOUS }
//   );
//   // Derived signals using computed that automatically update.
//   public isAuthenticated = computed(() => this.session() !== null);
//   public isAnonymous = computed(() => this.session() === null);
//   public username = computed(() => {
//     const session = this.session();
//     return session ? session.find(c => c.type === 'name')?.value || null : null;
//   });
//   public logoutUrl = computed(() => {
//     const session = this.session();
//     return session ? session.find(c => c.type === 'bff:logout_url')?.value || null : null;
//   });
//
//
//   public getSession(ignoreCache: boolean = false): Observable<Session> {
//     if (!this.session$ || ignoreCache) {
//       this.session$ = this.http.get<Session>('bff/user').pipe(
//         catchError(err => of(ANONYMOUS)),
//         shareReplay(CACHE_SIZE)
//       );
//     }
//     return this.session$;
//   }
// }
//
// export interface Claim {
//   type: string;
//   value: string;
// }
// export type Session = Claim[] | null;


import {computed, inject, Injectable, signal} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {tap} from 'rxjs';
import {TokenService} from '../token/token.service';
export interface LoginRequest { email: string; password: string; }
export interface LoginResponse { token: string; email: string; username?: string; }

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly tokenService = inject(TokenService);

  // ۱. تعریف Signal ها برای State (بسیار تمیز)
  public readonly currentUser = signal<{ firstname: string } | null>(null);

  // ۲. یک Signal مشتق شده (Computed) برای چک کردن وضعیت لاگین
  public readonly isAuthenticated = computed(() => this.currentUser() !== null);

  // login(credentials: LoginRequest) {
  //   return this.http.post<LoginResponse>('/api/login', credentials).pipe(
  //     tap(response => {
  //       console.log('نتیجه لاگین:', response);
  //       // ذخیره توکن در کوکی (برای پشتیبانی از SSR)
  //       this.tokenService.setToken(response.token);
  //
  //       // آپدیت کردن Signal (کل UI بلافاصله آپدیت می‌شود)
  //       this.currentUser.set({email: response.email, username: response.username});
  //     })
  //   );
  // }

  logout() {
    this.tokenService.removeToken();
    this.currentUser.set(null);
  }
}
