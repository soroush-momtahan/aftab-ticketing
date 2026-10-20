import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { SessionService } from '../services/session/session.service';

export const permissionGuard = (...required: string[]): CanActivateFn => (_route, state) => {
  const session = inject(SessionService);
  const router = inject(Router);

  if (!session.isAuthenticated())
    return router.createUrlTree(['/sign-on'], { queryParams: { returnUrl: state.url } });

  return required.every(p => session.hasPermission(p)) || router.createUrlTree(['/forbidden']);
};
