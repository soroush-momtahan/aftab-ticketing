import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import {TokenService} from '../../services/token/token.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const tokenService = inject(TokenService);
  const token = tokenService.getToken();

  // اگر توکن وجود داشت، آن را به هدر درخواست اضافه می‌کنیم
  if (token) {
    const clonedReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
    return next(clonedReq);
  }

  return next(req);
};
