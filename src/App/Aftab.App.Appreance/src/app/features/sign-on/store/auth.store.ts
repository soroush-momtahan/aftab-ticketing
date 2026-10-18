// import { signalStore, withState, withMethods, patchState } from '@ngrx/signals';
// import { rxMethod } from '@ngrx/signals/rxjs-interop';
// import { pipe, delay, tap } from 'rxjs';
// import { SignOnInputModel } from '../sign-on';
// import { inject } from '@angular/core';
// import { SignOnService } from '../services/sign-on.service';
//
//
// export type AuthStep = 'PHONE' | 'METHOD_SELECT' | 'OTP' | 'PASSWORD_LOGIN' | 'SET_PASSWORD' | 'SET_NAME' | 'SUCCESS';
// type OtpStatus = 'IDLE' | 'LOADING' | 'SUCCESS' | 'ERROR';
//
// export const AuthStore = signalStore(
//   withState({
//     step: 'PHONE' as AuthStep,
//     phone: '',
//     isExistingUser: false,
//     otpStatus: 'IDLE' as OtpStatus,
//     loading: false,
//   }),
//   withMethods((store, signOnService = inject(SignOnService)) => ({
//     // شبیه‌سازی بررسی شماره موبایل با RxJS
//     checkPhone: rxMethod<string>(
//       pipe(
//         tap(() => patchState(store, { loading: true })),
//         delay(800), // شبیه‌سازی ریکوئست
//         tap((phone) => {
//           // فرض کنیم اگر با 0912 شروع شد کاربر قدیمی است
//           const exists = phone.startsWith('0912');
//           patchState(store, {
//             phone,
//             isExistingUser: exists,
//             step: exists ? 'METHOD_SELECT' : 'OTP',
//             loading: false,
//           });
//         }),
//       ),
//     ),
//
//     // شبیه‌سازی بررسی کد پیامکی
//     verifyOtp: rxMethod<string>(
//       pipe(
//         tap(() => patchState(store, { otpStatus: 'LOADING', loading: true })),
//         delay(1000),
//         tap((code) => {
//           if (code === '123456') {
//             patchState(store, { otpStatus: 'SUCCESS', loading: false });
//             // بعد از انیمیشن تایید سبز، به مرحله بعد برود
//             setTimeout(() => {
//               patchState(store, {
//                 step: store.isExistingUser() ? 'SUCCESS' : 'SET_PASSWORD',
//                 otpStatus: 'IDLE',
//               });
//             }, 1500);
//           } else {
//             patchState(store, { otpStatus: 'ERROR', loading: false });
//             // ریست کردن ارور بعد از انیمیشن خطا
//             setTimeout(() => patchState(store, { otpStatus: 'IDLE' }), 1000);
//           }
//         }),
//       ),
//     ),
//
//     signUp: rxMethod<SignOnInputModel>(
//       pipe(
//         tap(() => patchState(store, { loading: true })),
//         delay(1000),
//         tap((signOnInput) => {
//           if (signOnInput.firstName === 'سروش') {
//             patchState(store, { loading: false, step: 'SUCCESS' });
//           } else {
//             patchState(store, { loading: false, step: 'PHONE' });
//           }
//         }),
//       ),
//     ),
//
//     signIn: rxMethod<SignOnInputModel>(
//       pipe(
//         tap(() => patchState(store, { loading: true })),
//         delay(1000),
//         tap((signOnInput) => {
//           if (signOnInput.password === '3oroTrg78@') {
//             patchState(store, { loading: false, step: 'SUCCESS' });
//           } else {
//             patchState(store, { loading: false, step: 'PASSWORD_LOGIN' });
//           }
//         }),
//       ),
//     ),
//
//     setStep: (step: AuthStep) => patchState(store, { step }),
//   })),
// );

import { signalStore, withState, withMethods, patchState } from '@ngrx/signals';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { tapResponse } from '@ngrx/operators';
import { inject } from '@angular/core';
import { pipe, exhaustMap, tap, delay } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';

import { SignOnService, SignUpRequest } from '../services/sign-on.service';
import { ActivatedRoute, Router } from '@angular/router';
import { ProblemDetails } from '../../../core/models/errors/problem-details';
import { TokenService } from '../../../core/services/token/token.service';
import { UserService } from '../../../core/services/user/user.service';

export type AuthStep =
  'PHONE' | 'METHOD_SELECT' | 'OTP' | 'PASSWORD_LOGIN' | 'SET_PASSWORD' | 'SET_NAME' | 'SUCCESS';

export type RequestStatus = 'idle' | 'loading' | 'success' | 'error';

interface AuthState {
  step: AuthStep;
  phone: string;
  tempToken: string | null;
  status: RequestStatus;
  error: ProblemDetails | null;
}

const initialState: AuthState = {
  step: 'PHONE',
  phone: '',
  tempToken: null,
  status: 'idle',
  error: null,
};

export const AuthStore = signalStore(
  withState(initialState),
  withMethods(
    (
      store,
      signOnService = inject(SignOnService),
      tokenService = inject(TokenService),
      userService = inject(UserService),
      router = inject(Router),
      route = inject(ActivatedRoute),
    ) => ({
      switchToErrorStatus() {
        patchState(store, { status: 'error' });
      },
      // ۱. بررسی شماره موبایل و ارسال OTP
      checkPhone: rxMethod<string>(
        pipe(
          tap((phone) => patchState(store, { status: 'loading', error: null, phone })),
          exhaustMap((phone) =>
            signOnService.sendOtpForNewUser({ mobile: phone }).pipe(
              tapResponse({
                next: (res) => {
                  patchState(store, {
                    status: 'idle',
                    step: res.isExistingUser ? 'METHOD_SELECT' : 'OTP',
                  });
                  console.log(res);
                },
                error: (err: HttpErrorResponse) => {
                  patchState(store, {
                    status: 'error',
                    error: err.error as ProblemDetails,
                  });
                  console.log(err);
                },
              }),
            ),
          ),
        ),
      ),

      sendOtp: rxMethod<string>(
        pipe(
          tap((phone) => patchState(store, { status: 'loading', error: null, phone })),
          exhaustMap((phone) =>
            signOnService.sendOtpForExistUser(phone).pipe(
              tapResponse({
                next: (res) => {
                  patchState(store, {
                    status: 'idle',
                    step: 'OTP',
                  });
                  console.log(res);
                },
                error: (err: HttpErrorResponse) => {
                  patchState(store, {
                    status: 'error',
                    error: err.error as ProblemDetails,
                  });
                  console.log(err);
                },
              }),
            ),
          ),
        ),
      ),

      // ۲. تایید کد پیامکی
      // verifyOtp: rxMethod<string>(
      //   pipe(
      //     tap(() => patchState(store, { status: 'loading', error: null })),
      //     exhaustMap((otpCode) =>
      //       // استفاده از phone که در State ذخیره شده بود
      //       signOnService.verifyMobile(store.phone(), otpCode).pipe(
      //         tapResponse({
      //           next: (res) => {
      //             patchState(store, {
      //               status: 'success',
      //               tempToken: res.tempToken,
      //               // اگر کاربر جدید بود میره برای انتخاب رمز، در غیر اینصورت میره مرحله موفقیت (یا لاگین)
      //               step: res.isNewUser ? 'SET_PASSWORD' : 'SUCCESS',
      //             });
      //
      //             // اگر بک‌اند در این مرحله توکن اصلی رو برمی‌گردونه، اینجا باید توکن ست بشه
      //             // اما چون TempToken میده، فرض بر اینه که مسیر کاربر جدید ادامه داره
      //           },
      //           error: (err: HttpErrorResponse) =>
      //             patchState(store, {
      //               status: 'error',
      //               error: err.error as ProblemDetails,
      //             }),
      //         }),
      //       ),
      //     ),
      //   ),
      // ),

      verifyOtp: rxMethod<string>(
        pipe(
          // قبل از هر چیز وضعیت رو به حالت لودینگ می‌بریم
          tap(() => patchState(store, { status: 'loading', error: null })),

          exhaustMap((otpCode) =>
            signOnService.verifyMobile(store.phone(), otpCode).pipe(
              // هندل کردن ریسپانس با tapResponse (بدون Boilerplate)
              tapResponse({
                next: (res) => {
                  if (res.isNewUser) {
                    // کاربر جدید است: توکن برای مرحله ثبت‌نام موقت است و در استیت ذخیره می‌شود
                    patchState(store, {
                      status: 'success',
                      tempToken: res.tempToken,
                    });
                  } else {
                    // کاربر قدیمی است: توکن برگشتی در واقع همان JWT لاگین است
                    tokenService.setToken(res.tempToken);
                    patchState(store, {
                      status: 'success',
                    });
                    userService.setUser({ name: store.phone(), mobile: store.phone() });
                    const returnUrl = route.snapshot.queryParams['returnUrl'] || '/';
                    router.navigateByUrl(returnUrl);
                  }
                },
                error: (err: HttpErrorResponse) => {
                  // اگر کد اشتباه بود، وضعیت قرمز می‌شود و جریان متوقف می‌شود (اما rxMethod زنده می‌ماند)
                  patchState(store, {
                    status: 'error',
                    error: err.error as ProblemDetails,
                  });
                },
              }),

              // این خطوطِ پایین فقط و فقط زمانی اجرا می‌شوند که درخواست موفقیت‌آمیز بوده باشد (وارد بخش next شده باشد)

              // ۱ ثانیه مکث کاملاً ریکتیو برای نمایش انیمیشن سبز شدن
              delay(1000),

              // هدایت به مرحله بعد پس از اتمام مکث
              tap((res) => {
                patchState(store, {
                  status: 'idle',
                  step: res.isNewUser ? 'SET_PASSWORD' : 'SUCCESS',
                });
              }),
            ),
          ),
        ),
      ),

      // ۳. لاگین با پسورد
      signIn: rxMethod<string>(
        pipe(
          tap(() => patchState(store, { status: 'loading', error: null })),
          exhaustMap((password) =>
            signOnService.signInWithPassword({ mobile: store.phone(), password }).pipe(
              tapResponse({
                next: (res) => {
                  tokenService.setToken(res.token);
                  patchState(store, { status: 'success', step: 'SUCCESS' });
                  const returnUrl = route.snapshot.queryParams['returnUrl'] || '/';
                  router.navigateByUrl(returnUrl);
                },
                error: (err: HttpErrorResponse) => {
                  patchState(store, {
                    status: 'error',
                    error: err.error as ProblemDetails,
                  });
                  console.log(err);
                },
              }),
            ),
          ),
        ),
      ),

      // ۴. ثبت نام نهایی
      signUp: rxMethod<
        Omit<SignUpRequest, 'tempToken'>
      > // TempToken از State خوانده می‌شود
      (
        pipe(
          tap(() => patchState(store, { status: 'loading', error: null })),
          exhaustMap((formData) =>
            signOnService
              .signUp({
                ...formData,
                tempToken: store.tempToken()!, // ارسال توکن موقت از استور
              })
              .pipe(
                tapResponse({
                  next: (res) => {
                    tokenService.setToken(res.token);
                    patchState(store, {
                      status: 'success',
                      step: 'SUCCESS',
                    });
                    userService.setUser({ name: formData.firstName, mobile: store.phone() });
                    const returnUrl = route.snapshot.queryParams['returnUrl'] || '/';
                    router.navigateByUrl(returnUrl);
                  },
                  error: (err: HttpErrorResponse) => {
                    patchState(store, {
                      status: 'error',
                      error: err.error as ProblemDetails,
                    });
                    console.log(err);
                  },
                }),
              ),
          ),
        ),
      ),

      // متدهای کمکی
      setStep: (step: AuthStep) => patchState(store, { status: 'idle', step, error: null }),
      clearError: () => patchState(store, { error: null }),
      resetStatus: () => patchState(store, { status: 'idle' }),
    }),
  ),
);
