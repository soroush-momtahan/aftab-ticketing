import {inject} from '@angular/core';
import {CanActivateFn, Router} from '@angular/router';
import { TokenService } from '../services/token/token.service';

export const authGuard: CanActivateFn = (route, state) => {
  const tokenService = inject(TokenService);
  const router = inject(Router);

  // آیا کاربر لاگین است؟
  if (tokenService.getToken()) {
    return true; // اجازه عبور بده
  }
  // loginHint.open();
  // return false;
  return router.createUrlTree(['/login'], {
    queryParams: {returnUrl: state.url}
  });
};
