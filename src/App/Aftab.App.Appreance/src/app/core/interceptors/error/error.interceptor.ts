import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';
import { ProblemDetails } from '../../models/errors/problem-details';
import { ToastService } from '../../components/toast-container/services/toast.service';
import { inject } from '@angular/core';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const toastService = inject(ToastService); // در صورت نیاز به نمایش پیام‌های عمومی

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      // ۱. استانداردسازی خطا (تضمین اینکه همیشه ProblemDetails داریم)
      const problemDetails = parseToProblemDetails(error);

      // ۲. مدیریت خطاهای عمومی (Global Error Handling)
      handleGlobalErrors(problemDetails, toastService);

      // ۳. پرتاب مجدد خطای استاندارد شده به لایه پایین‌تر (Store)
      // توجه: ما دیگر HttpErrorResponse را برنمی‌گردانیم!
      return throwError(() => problemDetails);
    }),
  );
};

/**
 * این متد تضمین می‌کند که تحت هر شرایطی خروجی منطبق بر اینترفیس شماست
 */
function parseToProblemDetails(error: HttpErrorResponse): ProblemDetails {
  // الف: سرور به درستی خطای ProblemDetails استاندارد دات‌نت را برگردانده است
  if (error.error && typeof error.error === 'object' && 'title' in error.error) {
    return error.error as ProblemDetails;
  }

  // ب: خطاهای شبکه (مثل قطعی اینترنت یا در دسترس نبودن سرور CORS)
  if (error.status === 0) {
    return {
      type: 'https://tools.ietf.org/html/rfc7231#section-6.6.4',
      title: 'خطای ارتباط با سرور',
      status: 0,
      detail: 'ارتباط با سرور برقرار نشد. لطفا اتصال اینترنت خود را بررسی کنید.',
      traceId: 'client-side-error',
    };
  }

  // ج: خطاهای پیش‌بینی نشده (مثل زمانی که سرور رشته متنی برمی‌گرداند یا IIS/Nginx ارور می‌دهد)
  return {
    type: 'https://tools.ietf.org/html/rfc7231#section-6.6.1',
    title: 'خطای سیستمی ناشناخته',
    status: error.status || 500,
    detail: error.message || 'یک خطای غیرمنتظره رخ داد.',
    traceId: 'unknown',
  };
}

/**
 * خطاهایی که نیاز نیست Storeها نگرانشان باشند
 */
function handleGlobalErrors(problem: ProblemDetails, toastService: ToastService) {
  switch (problem.status) {
    case 401:
      // مثال: هدایت کاربر به صفحه لاگین
      toastService.error('نشست شما پایان یافته است. لطفا مجدد وارد شوید.');
      break;
    case 403:
      // مثال: خطای عدم دسترسی
      // toast-container.error('شما دسترسی لازم برای این عملیات را ندارید.');
      break;
    case 500:
      // نمایش ToastContainer عمومی برای خطاهای سرور (جلوگیری از نمایش TraceId به کاربر)
      toastService.error('خطایی در سرور رخ داد. تیم فنی در حال بررسی است.');
      break;
    case 502:
      // نمایش ToastContainer عمومی برای خطاهای سرور (جلوگیری از نمایش TraceId به کاربر)
      toastService.error('خطایی در سرور رخ داد. تیم فنی در حال بررسی است.');
      break;
    case 0:
      toastService.warning('خطایی در سرور رخ داد. تیم فنی در حال بررسی است.');
      break;

    // نکته مهم: خطای 400 (Bad Request) و Validation Error ها اینجا هندل نمی‌شوند!
    // زیرا خطای 400 مختص به فرم است و باید به Store و سپس Component برود تا زیر فیلد مربوطه نمایش داده شود.
  }
}
