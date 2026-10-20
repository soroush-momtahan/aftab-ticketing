import { InjectionToken, Signal, signal } from '@angular/core';

export interface TicketingUser {
  id: string | null;
  isAdmin: boolean;
}
export const TICKETING_USER = new InjectionToken<Signal<TicketingUser>>('TICKETING_USER', {
  providedIn: 'root',
  factory: () => signal<TicketingUser>({ id: null, isAdmin: true }),
});
