import { Component, input, model, output, signal } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TicketStatus } from '../../../../enums/ticket-status.enum';
import {form, FormField, maxLength, minLength, required } from '@angular/forms/signals';
export interface NewReplyInput {
  content: string;
  status: TicketStatus | null;
}

@Component({
  imports: [ReactiveFormsModule, FormField],
  selector: 'app-new-reply',
  styleUrl: './new-reply.css',
  templateUrl: './new-reply.html',
})
export class NewReply {
  isAdmin = input.required<boolean>();
  isSubmitting = input.required<boolean>();
  replySentEvent = output<NewReplyInput>();
  input = signal<NewReplyInput>({
    content: '',
    status: null,
  });
  form = form(this.input, (form) => {
    // description
    required(form.content, { message: 'وارد کردن توضیحات الزامی است' });
    minLength(form.content, 6, { message: 'توضیحات وارد شده خیلی کوتاه هست' });
    maxLength(form.content, 3000, { message: 'توضیحات وارد شده خیلی طولانی است.' });
    // status
    required(form.status, { message: 'وارد کردن وضعیت تیکت الزامی است' });
  });

  statusOptions(): { value: TicketStatus; label: string }[] {
    return [
      { value: 1, label: 'باز' },
      { value: 2, label: 'در حال بررسی' },
      { value: 3, label: 'حل شده' },
      { value: 4, label: 'بسته' },
    ];
  }
  onSelectStatus(status: TicketStatus) {
    if (!this.isAdmin()) return;
    this.input.update((form) => ({ ...form, status }));
  }

  onSendReply() {
    if (this.form().invalid()) return;
    this.replySentEvent.emit(this.input());
  }
}
