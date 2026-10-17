import {
  Component,
  ElementRef,
  signal,
  viewChildren,
  inject,
  computed,
  effect,
  afterNextRender,
  PLATFORM_ID,
  ViewChild,
  viewChild,
} from '@angular/core';
import {
  FormField,
  form,
  required,
  minLength,
  maxLength,
  validate,
  pattern,
} from '@angular/forms/signals';
import { AuthStore } from './store/auth.store';
import {
  combineLatest,
  combineLatestWith,
  debounceTime,
  delay,
  filter,
  interval,
  takeWhile,
} from 'rxjs';
import { isPlatformBrowser } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from '../../core/services/user/user.service';
import { TokenService } from '../../core/services/token/token.service';
type Step = 'mobile' | 'verify-method' | 'otp' | 'password' | 'profile' | 'success';

export interface SignOnInputModel {
  phone: string;
  password: string;
  confirmPassword: string;
  firstName: string;
  lastName: string;
}

// function createSignOnStore() {
//   const state = signal<UserState>({ ...initialState });
//
//   return {
//     state: state.asReadonly(),
//     patch: (partial: Partial<UserState>) => state.update((s) => ({ ...s, ...partial })),
//     resetOtp: () =>
//       state.update((s) => ({
//         ...s,
//         otp: ['', '', '', '', '', ''],
//         otpError: false,
//         otpSuccess: false,
//       })),
//     setOtpDigit: (index: number, value: string) =>
//       state.update((s) => {
//         const otp = [...s.otp];
//         otp[index] = value;
//         return { ...s, otp };
//       }),
//   };
// }

/** شماره موبایل ایران: 11 رقم و شروع با 09 */
const IR_MOBILE_REGEX = /^09\d{9}$/;

/** حروف فارسی (شامل پ چ ژ گ ک ی) – بدون ارقام و علائم */
const PERSIAN_LETTERS =
  'آابپتثجچحخدذرزژسشصضطظعغفقکگلمنوهیءئؤأةكي';
const PERSIAN_LETTER_SET = new Set(PERSIAN_LETTERS);

/** نام فقط می‌تواند شامل حروف فارسی، فاصله و نیم‌فاصله باشد */
const PERSIAN_NAME_REGEX = new RegExp(`^[${PERSIAN_LETTERS}\\s\\u200C]+$`);

/** فقط حروف انگلیسی، عدد و کاراکتر ویژه */
const PASSWORD_ALLOWED_REGEX = /^[A-Za-z0-9!@#$%^&*()_+\-=[\]{};':"\\|,.<>/?~`]+$/;
const HAS_DIGIT = /\d/;
const HAS_UPPERCASE = /[A-Z]/;
const HAS_SPECIAL = /[!@#$%^&*()_+\-=[\]{};':"\\|,.<>/?~`]/;

/** تعداد حروف فارسی (بدون فاصله و نیم‌فاصله) */
const countPersianLetters = (value: string): number =>
  [...value].filter((ch) => PERSIAN_LETTER_SET.has(ch)).length;

const validationError = (error: { kind: string; message: string }) => error;

@Component({
  selector: 'app-sign-on',
  standalone: true,
  imports: [FormField, FormsModule],
  providers: [AuthStore],
  templateUrl: './sign-on.html',
  styleUrl: './sign-on.css',
})
export class SignOn {
  // readonly store = inject(AuthStore);
  //
  // /** 6 OTP inputs for sequential focus (client-side only after user interaction) */
  // otpInputs = viewChildren<ElementRef<HTMLInputElement>>('otpInput');
  //
  // loginModel = signal<SignOnInputModel>({
  //   phone: '',
  //   password: '',
  //   confirmPassword: '',
  //   firstName: '',
  //   lastName: '',
  // });
  //
  // loginForm = form(this.loginModel);
  //
  // otpCode = signal(['', '', '', '', '', '']);
  //
  // /** Clean phone value for validation */
  // phoneValue = computed(() => (this.loginForm.phone().value() ?? '').replace(/\s/g, ''));
  //
  // onSubmitPhone(): void {
  //   const phone = this.phoneValue();
  //   if (phone.length >= 10) {
  //     this.store.checkPhone(phone);
  //   }
  // }
  //
  // onOtpInput(event: Event, index: number): void {
  //   const input = event.target as HTMLInputElement;
  //   const val = input.value.replace(/\D/g, '').slice(0, 1); // digits only
  //
  //   const current = [...this.otpCode()];
  //   current[index] = val;
  //   this.otpCode.set(current);
  //
  //   // Auto-advance focus (safe: only after user keystroke)
  //   if (val && index < 5) {
  //     this.otpInputs()[index + 1]?.nativeElement.focus();
  //   }
  //
  //   // Auto-submit when complete
  //   if (index === 5 && current.every((c) => c !== '')) {
  //     this.store.verifyOtp(current.join(''));
  //   }
  // }
  //
  // onOtpKeydown(event: KeyboardEvent, index: number): void {
  //   if (event.key === 'Backspace' && !this.otpCode()[index] && index > 0) {
  //     this.otpInputs()[index - 1]?.nativeElement.focus();
  //   }
  // }
  //
  // trackByIndex(index: number): number {
  //   return index;
  // }

  readonly store = inject(AuthStore);
  readonly userService = inject(UserService);
  readonly tokenService = inject(TokenService);
  private platformId = inject(PLATFORM_ID);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  otpCode = signal(['', '', '', '', '', '']);



  signOnInputModel = signal<SignOnInputModel>({
    phone: '',
    password: '',
    confirmPassword: '',
    firstName: '',
    lastName: '',
  });

  signOnForm = form(this.signOnInputModel, (path) => {
    // ===== شماره موبایل =====
    required(path.phone, { message: 'وارد کردن شماره موبایل الزامی است.' });
    minLength(path.phone, 11, { message: 'شماره موبایل باید دقیقا 11 رقم باشد.' });
    maxLength(path.phone, 11, { message: 'شماره موبایل باید دقیقا 11 رقم باشد.' });
    pattern(path.phone, IR_MOBILE_REGEX, {
      message: 'شماره موبایل باید با 09 شروع شود و فقط شامل اعداد انگلیسی باشد.',
    });

    // ===== رمز عبور =====
    required(path.password, { message: 'وارد کردن رمز عبور الزامی است.' });
    minLength(path.password, 8, { message: 'رمز عبور باید حداقل 8 کاراکتر باشد.' });
    maxLength(path.password, 128, {
      message: 'تعداد کاراکترهای رمز عبور وارد شده بیش از حد مجاز است.',
    });
    pattern(path.password, PASSWORD_ALLOWED_REGEX, {
      message:
        'رمز عبور تنها می‌تواند شامل عدد، حروف بزرگ و کوچک انگلیسی و کاراکترهای ویژه (مانند !@#$%) باشد.',
    });

    validate(path.password, ({ value }) => {
      const v = value();
      if (!v) return null; // خطای required قبلاً اعمال می‌شود

      if (new Set(v).size < 4) {
        return validationError({
          kind: 'passwordDistinctChars',
          message: 'رمز عبور باید حداقل 4 کاراکتر متفاوت داشته باشد.',
        });
      }
      if (!HAS_DIGIT.test(v)) {
        return validationError({
          kind: 'passwordDigit',
          message: 'رمز عبور باید حداقل شامل یک عدد باشد.',
        });
      }
      if (!HAS_UPPERCASE.test(v)) {
        return validationError({
          kind: 'passwordUppercase',
          message: 'رمز عبور باید حداقل دارای یک حرف بزرگ انگلیسی باشد.',
        });
      }
      if (!HAS_SPECIAL.test(v)) {
        return validationError({
          kind: 'passwordSpecial',
          message: 'رمز عبور باید شامل حداقل یک کاراکتر ویژه (مانند !@#$%) باشد.',
        });
      }
      return null;
    });

    // ===== تکرار رمز عبور =====
    required(path.confirmPassword, { message: 'تکرار رمز عبور الزامی است.' });
    validate(path.confirmPassword, ({ value, valueOf }) => {
      const confirm = value();
      if (!confirm) return null;
      return confirm !== valueOf(path.password)
        ? validationError({
            kind: 'passwordMismatch',
            message: 'تکرار رمز عبور با رمز عبور یکسان نیست.',
          })
        : null;
    });

    // ===== نام =====
    required(path.firstName, { message: 'وارد کردن نام الزامی است.' });
    pattern(path.firstName, PERSIAN_NAME_REGEX, {
      message: 'نام باید فقط شامل حروف فارسی باشد.',
    });
    validate(path.firstName, ({ value }) => {
      const v = value();
      if (!v || !PERSIAN_NAME_REGEX.test(v)) return null;
      return countPersianLetters(v) < 3
        ? validationError({
            kind: 'firstNameMinLetters',
            message: 'نام باید حداقل شامل 3 حرف فارسی باشد.',
          })
        : null;
    });

    // ===== نام خانوادگی =====
    required(path.lastName, { message: 'وارد کردن نام خانوادگی الزامی است.' });
    pattern(path.lastName, PERSIAN_NAME_REGEX, {
      message: 'نام خانوادگی باید فقط شامل حروف فارسی باشد.',
    });
    validate(path.lastName, ({ value }) => {
      const v = value();
      if (!v || !PERSIAN_NAME_REGEX.test(v)) return null;
      return countPersianLetters(v) < 3
        ? validationError({
            kind: 'lastNameMinLetters',
            message: 'نام خانوادگی باید حداقل شامل 3 حرف فارسی باشد.',
          })
        : null;
    });
  });

  // View queries for OTP focus management
  otpInputs = viewChildren<ElementRef<HTMLInputElement>>('otpInput');
  verifyPhoneBtn = viewChild<ElementRef<HTMLButtonElement>>('verifyPhoneBtn');

  constructor() {
    effect(() => {
      console.log(`authenticate : ${this.userService.user()?.name}`);
      if (this.userService.user()) {
        const returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
        this.router.navigateByUrl(returnUrl);
      }
    });

    // Auto-focus first OTP box when entering OTP step (SSR-safe)
    effect(() => {
      const inputs = this.otpInputs();
      if (this.store.step() === 'OTP' && isPlatformBrowser(this.platformId)) {
        this.otpInputs()[0]?.nativeElement.focus();
      }
    });
    effect(() => {
      if (this.store.step() === 'OTP' && this.store.status() === 'error') {
        setTimeout(() => {
          this.otpInputs().forEach((input) => {
            input.nativeElement.value = '';
          });
          this.store.resetStatus();
          this.otpInputs()[0]?.nativeElement.focus();
        }, 800);
      }
    });

    // effect(() => {
    //   if (this.store.step() === 'OTP' && this.store.error()) {
    //     setTimeout(() => {
    //       this.otpInputs().forEach((input) => {
    //         input.nativeElement.value = '';
    //       });
    //       this.otpInputs()[0]?.nativeElement.focus();
    //     }, 800);
    //   }
    // })
  }

  otpBoxClass(i: number): string {
    const s = this.store.status();
    if (this.store.step() === 'OTP' && s === 'success')
      return 'border-emerald-500/60 bg-emerald-500/10 text-emerald-300 shadow-[0_0_20px_rgba(16,185,129,0.25)]';
    if (this.store.step() === 'OTP' && s === 'error')
      return 'border-red-500/60 bg-red-500/10 text-red-300 shadow-[0_0_20px_rgba(239,68,68,0.25)]';
    return 'border-white/15 focus:border-porcelain-500/70 focus:ring-2 focus:ring-porcelain-500/25 focus:shadow-[0_0_24px_rgba(227,171,28,0.2)]';
  }

  onCheckPhone() {
    this.store.checkPhone(this.signOnForm.phone().value());
  }

  onVerifyMethod(method: 'password' | 'otp') {
    if (method === 'otp') {
      this.store.sendOtp(this.signOnForm().value().phone);
      this.startResendTimer();
    } else {
      this.store.setStep('PASSWORD_LOGIN');
    }
  }

  // ───────────── OTP Logic (LTR + auto advance + backspace) ─────────────
  onOtpInput(event: Event, index: number) {
    const input = event.target as HTMLInputElement;
    const val = input.value.replace(/\D/g, '').slice(0, 1); // digits only

    const current = [...this.otpCode()];
    current[index] = val;
    this.otpCode.set(current);

    // Auto-advance focus (safe: only after user keystroke)
    if (val && index < 5) {
      this.otpInputs()[index + 1]?.nativeElement.focus();
    }

    // Auto-submit when complete
    if (index === 5 && current.every((c) => c !== '')) {
      this.store.verifyOtp(current.join(''));
    }
  }

  onOtpKeydown(event: KeyboardEvent, index: number): void {
    if (event.key === 'Backspace' && !this.otpCode()[index] && index > 0) {
      this.otpInputs()[index - 1]?.nativeElement.focus();
    }
  }
  onPhoneKeyDown(event: KeyboardEvent): void {
    if (event.key === 'Enter') {
      this.verifyPhoneBtn()?.nativeElement.click();
    }
  }

  onOtpPaste(event: ClipboardEvent) {
    // event.preventDefault();
    // const paste = event.clipboardData?.getData('text').replace(/\D/g, '').slice(0, 6) ?? '';
    // paste.split('').forEach((char, i) => this.store.setOtpDigit(i, char));
    // if (paste.length === 6) this.verifyOtp(paste);
    // else {
    //   const nextIndex = Math.min(paste.length, 5);
    //   this.otpInputs()[nextIndex]?.nativeElement.focus();
    // }
  }

  private startResendTimer() {
    // this.store.patch({ resendSeconds: 60 });
    // if (!isPlatformBrowser(this.platformId)) return;
    //
    // interval(1000)
    //   .pipe(takeWhile(() => this.store.state().resendSeconds > 0))
    //   .subscribe(() => {
    //     this.store.patch({ resendSeconds: this.store.state().resendSeconds - 1 });
    //   });
  }

  onResendOtp() {
    // this.store.resetOtp();
    // this.startResendTimer();
    // // Real: call SMS endpoint
  }

  onSetPassword() {
    this.store.switchToErrorStatus();
    if (this.signOnForm.password().invalid() || this.signOnForm.confirmPassword().invalid()) return;
    this.store.setStep('SET_NAME');
  }

  onSignUp() {
    this.store.switchToErrorStatus();
    if (this.signOnForm.firstName().invalid() || this.signOnForm.lastName().invalid()) return;
    this.store.signUp(this.signOnForm().value());
  }

  onSignIn() {
    this.store.signIn(this.signOnForm().value().password);
  }

  onSendOtp(){
    this.store.sendOtp(this.signOnForm().value().phone);
  }
}
