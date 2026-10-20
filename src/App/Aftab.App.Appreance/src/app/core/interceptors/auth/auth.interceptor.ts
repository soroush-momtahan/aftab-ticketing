import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import {TokenService} from '../../services/token/token.service';
import { SessionService } from '../../services/session/session.service';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

// export const authInterceptor: HttpInterceptorFn = (req, next) => {
//   const tokenService = inject(TokenService);
//   const token = tokenService.getToken();
//
//   // اگر توکن وجود داشت، آن را به هدر درخواست اضافه می‌کنیم
//   if (token) {
//     const clonedReq = req.clone({
//       setHeaders: {
//         Authorization: `Bearer ${token}`
//       }
//     });
//     return next(clonedReq);
//   }
//
//   return next(req);
// };

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = inject(TokenService).getToken();
  const session = inject(SessionService);
  const router = inject(Router);

  const authReq =
    token && req.url.startsWith('/api')
      ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
      : req;

  return next(authReq).pipe(
    catchError((err: HttpErrorResponse) => {
      if (err.status === 401 && session.isAuthenticated()) {
        session.logout();
        router.navigate(['/sign-on'], { queryParams: { returnUrl: router.url } });
      }
      return throwError(() => err);
    }),
  );
};
