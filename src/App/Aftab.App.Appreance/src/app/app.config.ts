import {ApplicationConfig, inject, provideBrowserGlobalErrorListeners, provideAppInitializer} from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './core/interceptors/auth/auth.interceptor';
import { errorInterceptor } from './core/interceptors/error/error.interceptor';
import {SessionService} from './core/services/session/session.service';
import { firstValueFrom } from 'rxjs';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideAppInitializer(() => {
      const session = inject(SessionService);
      return firstValueFrom(session.refresh());
    }),
    provideHttpClient(withInterceptors([authInterceptor, errorInterceptor])),
  ],
};
