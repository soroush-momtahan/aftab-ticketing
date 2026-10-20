// import { Component, input, model, output, signal } from '@angular/core';
// import { ReactiveFormsModule } from '@angular/forms';
// import { TicketStatus } from '../../../../enums/ticket-status.enum';
// import {form, FormField, maxLength, minLength, required } from '@angular/forms/signals';
// export interface NewReplyInput {
//   content: string;
//   status: TicketStatus | null;
// }
//
// @Component({
//   imports: [ReactiveFormsModule, FormField],
//   selector: 'app-new-reply',
//   styleUrl: './new-reply.css',
//   templateUrl: './new-reply.html',
// })
// export class NewReply {
//   isAdmin = input.required<boolean>();
//   isSubmitting = input.required<boolean>();
//   replySentEvent = output<NewReplyInput>();
//   input = signal<NewReplyInput>({
//     content: '',
//     status: null,
//   });
//   form = form(this.input, (form) => {
//     // description
//     required(form.content, { message: 'وارد کردن توضیحات الزامی است' });
//     minLength(form.content, 6, { message: 'توضیحات وارد شده خیلی کوتاه هست' });
//     maxLength(form.content, 3000, { message: 'توضیحات وارد شده خیلی طولانی است.' });
//     // status
//     required(form.status, { message: 'وارد کردن وضعیت تیکت الزامی است' });
//   });
//
//   statusOptions(): { value: TicketStatus; label: string }[] {
//     return [
//       { value: 1, label: 'باز' },
//       { value: 2, label: 'در حال بررسی' },
//       { value: 3, label: 'حل شده' },
//       { value: 4, label: 'بسته' },
//     ];
//   }
//   onSelectStatus(status: TicketStatus) {
//     if (!this.isAdmin()) return;
//     this.input.update((form) => ({ ...form, status }));
//   }
//
//   onSendReply() {
//     if (this.form().invalid()) return;
//     this.replySentEvent.emit(this.input());
//   }
// }

// مسیر: ticketing/pages/ticket-detail/components/new-reply/new-reply.ts
import { Component, computed, effect, input, linkedSignal, output, signal } from '@angular/core';
import { form, FormField, maxLength, minLength, required } from '@angular/forms/signals';
import { TicketStatus } from '../../../../enums/ticket-status.enum';
import { STATUS_OPTIONS } from '../../../../mappers/ticket.mappers';
import { SubmitState } from '../../../../store/ticketing.store';

export interface NewReplyInput {
  content: string;
  status: TicketStatus;
}

@Component({
  selector: 'app-new-reply',
  imports: [FormField],
  styleUrl: './new-reply.css',
  templateUrl: './new-reply.html',
})
export class NewReply {
  readonly isAdmin = input.required<boolean>();
  readonly currentStatus = input.required<TicketStatus>();
  readonly submitState = input<SubmitState>('idle');
  readonly replySentEvent = output<NewReplyInput>();

  protected readonly statusOptions = STATUS_OPTIONS;
  protected readonly isSubmitting = computed(() => this.submitState() === 'pending');

  // وضعیت انتخابی ادمین. پیش‌فرض = وضعیت فعلی تیکت و با هر تغییر آن (مثلا بعد از ثبت پاسخ) دوباره هم‌گام می‌شود.
  // برای کاربر عادی همین مقدار (بدون تغییر وضعیت) ارسال می‌شود، پس دیگر نیازی به required روی status نیست.
  protected readonly status = linkedSignal(() => this.currentStatus());

  private readonly model = signal({ content: '' });
  protected readonly form = form(this.model, (f) => {
    required(f.content, { message: 'وارد کردن توضیحات الزامی است' });
    minLength(f.content, 6, { message: 'توضیحات وارد شده خیلی کوتاه هست' });
    maxLength(f.content, 3000, { message: 'توضیحات وارد شده خیلی طولانی است.' });
  });

  constructor() {
    // بعد از ثبت موفق، فرم پاک می‌شود (در صورت خطا متن کاربر حفظ می‌شود)
    effect(() => {
      if (this.submitState() === 'success') this.model.set({ content: '' });
    });
  }

  protected send() {
    if (this.form().invalid() || this.isSubmitting()) return;
    this.replySentEvent.emit({ content: this.model().content.trim(), status: this.status() });
  }
}
