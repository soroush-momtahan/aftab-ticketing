import { ProblemDetails } from '../../models/errors/problem-details';
import { computed } from '@angular/core';
import { signalStoreFeature, withComputed, withState } from '@ngrx/signals';

export type RequestStatus = 'idle' | 'loading' | 'success' | 'error';

export interface RequestStatusState {
  status: RequestStatus;
  error: ProblemDetails | null;
}

const initialRequestStatusState: RequestStatusState = {
  status: 'idle',
  error: null,
};

export function withRequestStatus() {
  return signalStoreFeature(
    withState(initialRequestStatusState),
    withComputed(({ status, error }) => ({
      isLoading: computed(() => status() === 'loading'),
      isSuccess: computed(() => status() === 'success'),
      isError: computed(() => status() === 'error'),
      hasError: computed(() => error() !== null),
    })),
  );
}

// توابع کمکی برای آپدیت State (کاهش بیشتر Boilerplate)
export function setPending(): RequestStatusState {
  return { status: 'loading', error: null };
}
export function setFulfilled(): RequestStatusState {
  return { status: 'success', error: null };
}
export function setRejected(error: ProblemDetails): RequestStatusState {
  return { status: 'error', error };
}
