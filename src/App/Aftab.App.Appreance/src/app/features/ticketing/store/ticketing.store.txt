import { ProblemDetails } from '../../../core/models/errors/problem-details';
import { patchState, signalStore, withMethods, withState } from '@ngrx/signals';
import { AddTicketRequest, TicketingService } from '../services/ticketing.service';
import { inject } from '@angular/core';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { exhaustMap, pipe, tap } from 'rxjs';
import { tapResponse } from '@ngrx/operators';
import {
  setFulfilled,
  setPending,
  setRejected,
  withRequestStatus,
} from '../../../core/store/features/request-status.festure';

interface TicketingState {
  currentStep: number,
}

const initialState: TicketingState = {
  currentStep: 1,
}

export const TicketingStore = signalStore(
  withState(initialState),
  withRequestStatus(),
  withMethods((store, ticketingService = inject(TicketingService)) => ({
    nextStep() {
      patchState(store, (state) => ({
        currentStep: state.currentStep < 5 ? state.currentStep + 1 : state.currentStep,
      }));
    },

    prevStep() {
      patchState(store, (state) => ({
        currentStep: state.currentStep > 1 ? state.currentStep - 1 : state.currentStep,
      }));
    },
    addTicket: rxMethod<AddTicketRequest>(
      pipe(
        tap(() => patchState(store, setPending())),
        exhaustMap((req) =>
          ticketingService.AddTicket(req).pipe(
            tapResponse({
              next: () => {
                patchState(store, {
                  currentStep: 5,
                }, setFulfilled());
              },
              error: (err: ProblemDetails) => {
                patchState(store, setRejected(err));
              },
            }),
          ),
        ),
      ),
    ),
  })),
);

