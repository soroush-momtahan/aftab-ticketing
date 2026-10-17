import { ProblemDetails } from '../../../core/models/errors/problem-details';
import { patchState, signalStore, withMethods, withState } from '@ngrx/signals';
import { AddTicketRequest, TicketingService } from '../services/ticketing.service';
import { inject } from '@angular/core';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { exhaustMap, pipe, tap } from 'rxjs';
import { tapResponse } from '@ngrx/operators';
import { HttpErrorResponse } from '@angular/common/http';

export type Status = 'idle' | 'loading' | 'success' | 'error'
interface TicketingState {
  currentStep: number;
  status: Status;
  error: ProblemDetails | null
}

const initialState: TicketingState = {
  currentStep: 1,
  status: 'idle' as Status,
  error: null,
}

export const TicketingStore = signalStore(
  withState(initialState),
  withMethods((store, ticketingService = inject(TicketingService)) => ({
    nextStep() {
      patchState(store, (state) => ({
        currentStep: state.currentStep < 5 ? state.currentStep + 1 : state.currentStep,
        status: 'idle' as Status,
        error: null,
      }));
    },

    prevStep() {
      patchState(store, (state) => ({
        currentStep: state.currentStep > 1 ? state.currentStep - 1 : state.currentStep,
        status: 'idle' as Status,
        error: null,
      }));
    },
    addTicket: rxMethod<AddTicketRequest>(
      pipe(
        tap(() => patchState(store, { status: 'loading' })),
        exhaustMap((req) =>
          ticketingService.AddTicket(req).pipe(
            tapResponse({
              next: () => {
                patchState(store, {
                  currentStep: 5,
                  status: 'success',
                });
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
  })),
);

