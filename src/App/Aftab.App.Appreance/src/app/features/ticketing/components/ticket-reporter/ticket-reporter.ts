import { Component, computed, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { RouterLink } from '@angular/router';
import { catchError, finalize, of } from 'rxjs';

type TicketStatus = 'Open' | 'InProgress' | 'Answered' | 'Closed';

interface TicketReporterStatusView {
  ticketId: string;
  status: TicketStatus;
}

interface TicketReporterView {
  id: string;
  organId: string;
  organName: string;
  statuses?: TicketReporterStatusView[];
  totalStatusCount: number;
  openTickets: number;
  inprogressTickets: number;
  answeredTickets: number;
  closedTickets: number;
}

interface TicketReporterResponse {
  ticketReports: TicketReporterView[];
}

@Component({
  selector: 'app-ticket-reporter',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './ticket-reporter.html',
  styleUrl: './ticket-reporter.css',
})
export class TicketReporter {
  private readonly http = inject(HttpClient);

  readonly isLoading = signal(true);
  readonly reports = signal<TicketReporterView[]>([]);
  readonly error = signal<string | null>(null);

  // ========== GLOBAL METRICS ==========
  readonly globalMetrics = computed(() => {
    const list = this.reports();
    const open = list.reduce((s, r) => s + r.openTickets, 0);
    const inProgress = list.reduce((s, r) => s + r.inprogressTickets, 0);
    const answered = list.reduce((s, r) => s + r.answeredTickets, 0);
    const closed = list.reduce((s, r) => s + r.closedTickets, 0);

    return [
      { key: 'open', label: 'باز', value: open, colorClass: 'text-[#e3ab1c]' },
      { key: 'inprogress', label: 'در حال انجام', value: inProgress, colorClass: 'text-blue-300' },
      { key: 'answered', label: 'پاسخ‌داده‌شده', value: answered, colorClass: 'text-emerald-300' },
      { key: 'closed', label: 'بسته', value: closed, colorClass: 'text-[#a3a3a3]' },
    ];
  });

  constructor() {
    this.loadReports();
  }

  private loadReports(): void {
    this.isLoading.set(true);
    this.error.set(null);

    this.http
      .get<TicketReporterResponse>('/api/tickets/reports')
      .pipe(
        catchError((err) => {
          console.error('Failed to load ticket reports', err);
          this.error.set('خطا در دریافت گزارش تیکت‌ها');
          return of({ ticketReports: [] });
        }),
        finalize(() => this.isLoading.set(false)),
      )
      .subscribe((res) => {
        this.reports.set(res.ticketReports ?? []);
      });
  }

  percent(count: number, total: number): number {
    if (total === 0) return 0;
    return Math.round((count / total) * 100);
  }
}
