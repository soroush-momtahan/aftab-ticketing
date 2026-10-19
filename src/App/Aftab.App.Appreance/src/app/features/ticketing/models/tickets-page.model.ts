// مسیر: ticketing/services/tickets-page.model.ts

import { GetTicketResponse } from '../services/ticketing.service';

/**
 * خروجی لیست تیکت‌ها (صفحه‌بندی سمت سرور).
 * سرور باید totalCount را هم برگرداند تا UI بتواند تعداد صفحات را حساب کند.
 */
export interface TicketsPage {
  pageIndex: number;
  pageSize: number;
  count: number;
  data: GetTicketResponse[];
}
