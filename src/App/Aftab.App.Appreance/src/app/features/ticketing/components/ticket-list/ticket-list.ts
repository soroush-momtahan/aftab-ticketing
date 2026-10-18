import { Component, computed, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

type TicketStatus = 'open' | 'in_progress' | 'resolved' | 'closed';
type TicketDepartment = 'support' | 'sales' | 'technical';
type Priority = 'low' | 'medium' | 'high';

interface TicketSummary {
  id: string;
  title: string;
  status: TicketStatus;
  department: TicketDepartment;
  priority: Priority;
  customerName: string;
  createdAt: string;
  updatedAt: string;
  repliesCount: number;
}

@Component({
  imports: [FormsModule, RouterLink],
  selector: 'app-ticket-list',
  styleUrl: './ticket-list.css',
  templateUrl: './ticket-list.html',
})
export class TicketList {
  // ========== FILTERS (mirrors GetTicketsRequest) ==========
  readonly searchString = signal('');
  readonly sortByPriority = signal(false);
  readonly filterDepartment = signal<TicketDepartment | null>(null);
  readonly filterStatus = signal<TicketStatus | null>(null);
  readonly filterByPersonId = signal<string | null>(null); // ready for future use
  readonly pageIndex = signal(1);
  readonly pageSize = signal(8);

  readonly isLoading = signal(false);

  // ========== MOCK DATA ==========
  private readonly allTickets = signal<TicketSummary[]>([
    {
      id: 'TK-24087',
      title: 'صدای لنت جلو بعد از تعویض',
      status: 'in_progress',
      department: 'technical',
      priority: 'high',
      customerName: 'علی رضایی',
      createdAt: '۱۴۰۳/۰۷/۱۲',
      updatedAt: '۱۴۰۳/۰۷/۱۳',
      repliesCount: 4,
    },
    {
      id: 'TK-24065',
      title: 'درخواست فاکتور رسمی برای خرید عمده',
      status: 'open',
      department: 'sales',
      priority: 'medium',
      customerName: 'شرکت خودروسازان پارس',
      createdAt: '۱۴۰۳/۰۷/۱۰',
      updatedAt: '۱۴۰۳/۰۷/۱۰',
      repliesCount: 1,
    },
    {
      id: 'TK-24051',
      title: 'لنت عقب زودتر از موعد تمام شد',
      status: 'resolved',
      department: 'support',
      priority: 'high',
      customerName: 'مریم احمدی',
      createdAt: '۱۴۰۳/۰۷/۰۵',
      updatedAt: '۱۴۰۳/۰۷/۰۸',
      repliesCount: 6,
    },
    {
      id: 'TK-24033',
      title: 'سوال در مورد سازگاری لنت با دیسک سوراخ‌دار',
      status: 'closed',
      department: 'technical',
      priority: 'low',
      customerName: 'حسین کریمی',
      createdAt: '۱۴۰۳/۰۶/۲۸',
      updatedAt: '۱۴۰۳/۰۷/۰۲',
      repliesCount: 3,
    },
    {
      id: 'TK-24019',
      title: 'پیگیری سفارش ارسال نشده',
      status: 'in_progress',
      department: 'sales',
      priority: 'medium',
      customerName: 'رضا موسوی',
      createdAt: '۱۴۰۳/۰۶/۲۰',
      updatedAt: '۱۴۰۳/۰۶/۲۲',
      repliesCount: 2,
    },
    {
      id: 'TK-23998',
      title: 'درخواست گارانتی تعویض لنت',
      status: 'open',
      department: 'support',
      priority: 'high',
      customerName: 'نازنین فرهادی',
      createdAt: '۱۴۰۳/۰۶/۱۵',
      updatedAt: '۱۴۰۳/۰۶/۱۵',
      repliesCount: 0,
    },
    {
      id: 'TK-23972',
      title: 'مشاوره انتخاب لنت برای پیست',
      status: 'resolved',
      department: 'technical',
      priority: 'medium',
      customerName: 'امیرحسین نوری',
      createdAt: '۱۴۰۳/۰۶/۰۸',
      updatedAt: '۱۴۰۳/۰۶/۱۲',
      repliesCount: 5,
    },
    {
      id: 'TK-23950',
      title: 'مشکل در کد تخفیف خرید اول',
      status: 'closed',
      department: 'sales',
      priority: 'low',
      customerName: 'سارا محمدی',
      createdAt: '۱۴۰۳/۰۵/۳۰',
      updatedAt: '۱۴۰۳/۰۶/۰۲',
      repliesCount: 2,
    },
  ]);

  // ========== OPTIONS ==========
  readonly departments = [
    { value: 'support' as const, label: 'پشتیبانی' },
    { value: 'sales' as const, label: 'فروش' },
    { value: 'technical' as const, label: 'فنی' },
  ];

  readonly statuses = [
    { value: 'open' as const, label: 'باز' },
    { value: 'in_progress' as const, label: 'در حال بررسی' },
    { value: 'resolved' as const, label: 'حل شده' },
    { value: 'closed' as const, label: 'بسته' },
  ];

  // ========== COMPUTED ==========
  readonly filteredTickets = computed(() => {
    let list = [...this.allTickets()];

    // Search
    const q = this.searchString().trim().toLowerCase();
    if (q) {
      list = list.filter(
        (t) =>
          t.title.toLowerCase().includes(q) ||
          t.id.toLowerCase().includes(q) ||
          t.customerName.toLowerCase().includes(q),
      );
    }

    // Department
    if (this.filterDepartment()) {
      list = list.filter((t) => t.department === this.filterDepartment());
    }

    // Status
    if (this.filterStatus()) {
      list = list.filter((t) => t.status === this.filterStatus());
    }

    // Sort by priority
    if (this.sortByPriority()) {
      const order: Record<Priority, number> = { high: 0, medium: 1, low: 2 };
      list.sort((a, b) => order[a.priority] - order[b.priority]);
    }

    return list;
  });

  readonly totalCount = computed(() => this.filteredTickets().length);
  readonly totalPages = computed(() => Math.max(1, Math.ceil(this.totalCount() / this.pageSize())));

  readonly paginatedTickets = computed(() => {
    const start = (this.pageIndex() - 1) * this.pageSize();
    return this.filteredTickets().slice(start, start + this.pageSize());
  });

  readonly startItem = computed(() =>
    this.totalCount() === 0 ? 0 : (this.pageIndex() - 1) * this.pageSize() + 1,
  );
  readonly endItem = computed(() =>
    Math.min(this.pageIndex() * this.pageSize(), this.totalCount()),
  );

  readonly visiblePages = computed(() => {
    const total = this.totalPages();
    const current = this.pageIndex();
    const pages: number[] = [];
    const range = 2;

    for (let i = Math.max(1, current - range); i <= Math.min(total, current + range); i++) {
      pages.push(i);
    }
    return pages;
  });

  // ========== ACTIONS ==========
  onSearch(value: string) {
    this.searchString.set(value);
    this.pageIndex.set(1);
  }

  setDepartment(value: TicketDepartment | null) {
    this.filterDepartment.set(value);
    this.pageIndex.set(1);
  }

  setStatus(value: TicketStatus | null) {
    this.filterStatus.set(value);
    this.pageIndex.set(1);
  }

  toggleSortByPriority() {
    this.sortByPriority.update((v) => !v);
    this.pageIndex.set(1);
  }

  goToPage(page: number) {
    if (page < 1 || page > this.totalPages()) return;
    this.pageIndex.set(page);
  }

  // ========== HELPERS ==========
  statusLabel(s: TicketStatus): string {
    const map: Record<TicketStatus, string> = {
      open: 'باز',
      in_progress: 'در حال بررسی',
      resolved: 'حل شده',
      closed: 'بسته',
    };
    return map[s];
  }

  statusBadgeClass(s: TicketStatus): string {
    const map: Record<TicketStatus, string> = {
      open: 'border-[#e3ab1c]/40 bg-[#e3ab1c]/10 text-[#e3ab1c]',
      in_progress: 'border-blue-400/40 bg-blue-400/10 text-blue-300',
      resolved: 'border-emerald-400/40 bg-emerald-400/10 text-emerald-300',
      closed: 'border-[#3a3a3a] bg-[#1a1a1a] text-[#888]',
    };
    return map[s];
  }

  statusDotClass(s: TicketStatus): string {
    const map: Record<TicketStatus, string> = {
      open: 'bg-[#e3ab1c]',
      in_progress: 'bg-blue-400',
      resolved: 'bg-emerald-400',
      closed: 'bg-[#555]',
    };
    return map[s];
  }

  departmentLabel(d: TicketDepartment): string {
    const map: Record<TicketDepartment, string> = {
      support: 'پشتیبانی',
      sales: 'فروش',
      technical: 'فنی',
    };
    return map[d];
  }

  priorityLabel(p: Priority): string {
    const map: Record<Priority, string> = {
      low: 'کم',
      medium: 'متوسط',
      high: 'زیاد',
    };
    return map[p];
  }

  priorityBadgeClass(p: Priority): string {
    const map: Record<Priority, string> = {
      low: 'border-[#2a2a2a] text-[#888]',
      medium: 'border-[#e9bb49]/40 text-[#e9bb49]',
      high: 'border-[#e3ab1c]/50 bg-[#e3ab1c]/10 text-[#e3ab1c]',
    };
    return map[p];
  }
}
