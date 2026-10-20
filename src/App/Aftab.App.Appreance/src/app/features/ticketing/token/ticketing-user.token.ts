import { computed, inject, InjectionToken, Signal, signal } from '@angular/core';
import { SessionService } from '../../../core/services/session/session.service';

export interface TicketingUser {
  id: string | null;
  isAdmin: boolean;
}
export const TICKETING_USER = new InjectionToken<Signal<TicketingUser>>('TICKETING_USER', {
  providedIn: 'root',
  factory: () => {
    const session = inject(SessionService);
    return computed(() => ({ id: session.session()?.id ?? null, isAdmin: session.isAdmin() }));
  },
});
