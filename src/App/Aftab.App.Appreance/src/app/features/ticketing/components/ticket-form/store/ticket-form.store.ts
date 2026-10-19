import { patchState, signalStore, withComputed, withMethods, withState } from '@ngrx/signals';
import { computed, inject } from '@angular/core';
import { AddTicketRequest } from '../../../services/ticketing.service';
import { TicketFormInputModel } from '../ticket-form';
import { TicketingStore } from '../../../store/ticketing.store';
import { TicketingStep } from '../../../enums/ticketing-step.enum';

interface TicketFormStore {
  currentStep: TicketingStep;
  prevStep: TicketingStep;
  maxSteps: TicketingStep;
}

const initialState: TicketFormStore = {
  currentStep: TicketingStep.ProblemDesc,
  prevStep: TicketingStep.ProblemDesc,
  maxSteps: TicketingStep.Confirmation,
};

export const mapToRequest = (formModel: TicketFormInputModel): AddTicketRequest => ({
  content: formModel.description,
  department: formModel.department,
  priority: formModel.priority,
  title: formModel.title,
});

export const TicketFormStore = signalStore(
  withState(initialState),
  withComputed((store, domainStore = inject(TicketingStore)) => ({
    progressPercent: computed(() => ((store.currentStep() - 1) / (store.maxSteps() - 1)) * 100),
    isSubmitting: computed(() => domainStore.isLoading()),
    isSuccess: computed(() => domainStore.isSuccess()),
    hasError: computed(() => domainStore.hasError()),
  })),
  withMethods((store, domainStore = inject(TicketingStore)) => ({
    nextStep() {
      patchState(store, (state) => ({
        currentStep: Math.min(state.currentStep + 1, state.maxSteps),
      }));
    },

    prevStep() {
      patchState(store, (state) => ({
        currentStep: Math.max(state.currentStep - 1, 1),
      }));
    },
    submitTicket(finalData: AddTicketRequest) {
      if (store.currentStep() !== store.maxSteps() - 1) return;
      patchState(store, () => ({currentStep: 5}))
      domainStore.addTicket(finalData);
    }
  })),
);
