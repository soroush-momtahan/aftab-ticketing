import { Component, computed, effect, inject, signal } from '@angular/core';
import { form, FormField, maxLength, minLength, required } from '@angular/forms/signals';
import { TicketDepartment } from '../../enums/ticket-department.enum';
import { TicketPriority } from '../../enums/ticket-priority.enum';
import { AddTicketRequest } from '../../services/ticketing.service';
import { mapToRequest, TicketFormStore } from './store/ticket-form.store';
import { TicketingStore } from '../../store/ticketing.store';
import { RouterLink } from '@angular/router';

export interface TicketFormInputModel {
  description: string;
  department: TicketDepartment | null;
  priority: TicketPriority | null;
  title: string;
}

@Component({
  imports: [FormField, RouterLink],
  selector: 'app-ticketing',
  providers: [TicketFormStore, TicketingStore],
  styleUrl: './ticket-form.css',
  templateUrl: './ticket-form.html',
})
export class TicketForm {
  readonly store = inject(TicketFormStore);

  ticketFormInputModel = signal<TicketFormInputModel>({
    description: '',
    department: null,
    priority: null,
    title: '',
  });

  form = form(this.ticketFormInputModel, (form) => {
    // description
    required(form.description, { message: 'وارد کردن توضیحات الزامی است' });
    minLength(form.description, 6, { message: 'توضیحات وارد شده خیلی کوتاه هست' });
    maxLength(form.description, 3000, { message: 'توضیحات وارد شده خیلی طولانی است.' });

    required(form.department, { message: 'لطفاً یک بخش را انتخاب کنید' });

    required(form.priority, { message: 'لطفاً یک اولویت را انتخاب کنید' });

    required(form.title, { message: 'وارد کردن عنوان الزامی است' });
    minLength(form.title, 3, { message: 'عنوان نباید کمتر از ۳ کاراکتر باشد' });
    maxLength(form.title, 100, { message: 'عنوان نباید بیشتر از ۱۰۰ کاراکتر باشد' });
  });

  readonly steps = [
    { id: 1, label: 'شرح مشکل' },
    { id: 2, label: 'بخش' },
    { id: 3, label: 'اولویت' },
    { id: 4, label: 'عنوان' },
    { id: 5, label: 'تأیید' },
  ];

  readonly departments = [
    {
      id: TicketDepartment.Support as TicketDepartment,
      label: 'پشتیبانی',
    },
    {
      id: TicketDepartment.Sale as TicketDepartment,
      label: 'فروش',
    },
    {
      id: TicketDepartment.Technical as TicketDepartment,
      label: 'فنی',
    },
    {
      id: TicketDepartment.Management as TicketDepartment,
      label: 'مدیریت',
    },
  ];

  readonly priorities = [
    { id: TicketPriority.Low as TicketPriority, label: 'کم', short: 'L', desc: 'غیرفوری' },
    { id: TicketPriority.Medium as TicketPriority, label: 'متوسط', short: 'M', desc: 'عادی' },
    { id: TicketPriority.High as TicketPriority, label: 'زیاد', short: 'H', desc: 'فوری' },
  ];

  canProceed = computed(() => {
    switch (this.store.currentStep()) {
      case 1:
        return this.form.description().valid();
      case 2:
        return this.form.department().valid();
      case 3:
        return this.form.priority().valid();
      case 4:
        return this.form.title().valid();
      case 5:
        return this.form().valid();
      default:
        return false;
    }
  });

  constructor() {
    effect(() => {
      if (this.store.isSuccess()) {
      }
    });
  }

  onSelectDepartment(ticketDepartment: TicketDepartment) {
    this.ticketFormInputModel.update((form) => ({ ...form, department: ticketDepartment }));
  }

  onSelectPriority(id: TicketPriority) {
    this.ticketFormInputModel.update((form) => ({ ...form, priority: id }));
  }

  onNextStep() {
    if (!this.canProceed()) return;
    this.store.nextStep();
  }

  onPrevStep() {
    this.store.prevStep();
  }

  onSubmit(event?: Event) {
    if (event) event.preventDefault();
    if (!this.canProceed()) return;
    const payload = mapToRequest(this.ticketFormInputModel());
    this.store.submitTicket(payload);
  }

  protected readonly TicketDepartment = TicketDepartment;
}
