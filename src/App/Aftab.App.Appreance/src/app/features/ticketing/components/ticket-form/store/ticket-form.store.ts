import { patchState, signalStore, withComputed, withMethods, withState } from '@ngrx/signals';
import { computed, inject } from '@angular/core';
import { AddTicketRequest } from '../../../services/ticketing.service';
import { TicketingStore } from '../../../store/ticketing.store';
import { TicketFormInputModel } from '../ticket-form';

interface TicketFormStore {
  currentStep: number;
  maxSteps: number;
}

const initialState: TicketFormStore = {
  currentStep: 1,
  maxSteps: 5,
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
    // ۲. الگوی Facade: بازتاب وضعیت لودینگ و خطا از Store اصلی بدون افشای خود Store اصلی
    isSubmitting: computed(() => domainStore.isLoading()),
    isSuccess: computed(() => domainStore.isSuccess()),
    hasError: computed(() => domainStore.hasError()),
  })),
  withMethods((store, domainStore = inject(TicketingStore)) => ({
    nextStep() {
      // فقط Step را مدیریت می‌کنیم، نیازی به کش کردن دیتا نیست
      patchState(store, (state) => ({
        currentStep: Math.min(state.currentStep + 1, state.maxSteps),
      }));
    },

    prevStep() {
      patchState(store, (state) => ({
        currentStep: Math.max(state.currentStep - 1, 1),
      }));
    },

    // حالا دیتای نهایی فقط موقع سابمیت به Store پاس داده می‌شود
    submitTicket(finalData: AddTicketRequest) {
      if (store.currentStep() !== store.maxSteps() - 1) return;
      patchState(store, () => ({currentStep: 5}))
      domainStore.addTicket(finalData);
    },

    resetForm() {
      patchState(store, initialState);
    },
  })),
);
