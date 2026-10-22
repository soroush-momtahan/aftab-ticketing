import { computed, inject } from '@angular/core';
import { signalStore, withComputed, withHooks, withMethods } from '@ngrx/signals';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { debounceTime, distinctUntilChanged, pipe, tap } from 'rxjs';
import { TicketingStore } from '../../../store/ticketing.store';
import { TicketStatus } from '../../../enums/ticket-status.enum';
import { TicketDepartment } from '../../../enums/ticket-department.enum';
import { TicketDto } from '../../../services/ticketing.service';
import {
  formatDate,
  getDepartmentLabel,
  getPriorityBadgeClass,
  getPriorityLabel,
  getStatusBadgeClass,
  getStatusDotClass,
  getStatusLabel,
} from '../../../mappers/ticket.mappers';

export interface TicketListViewModel extends TicketDto {
  statusLabel: string;
  statusBadgeClass: string;
  statusDotClass: string;
  priorityLabel: string;
  priorityBadgeClass: string;
  departmentLabel: string;
  createdAtLabel: string;
  repliesCount: number;
}

export const TicketListStore = signalStore(
  withComputed((_, domainStore = inject(TicketingStore)) => {
    const tickets = computed<TicketListViewModel[]>(() =>
      domainStore.entities().map((entity) => {
        const t = entity.ticket;
        return {
          ...t,
          repliesCount: entity.replies?.length ?? 0,

          statusLabel: getStatusLabel(t.status),
          statusBadgeClass: getStatusBadgeClass(t.status),
          statusDotClass: getStatusDotClass(t.status),

          priorityLabel: getPriorityLabel(t.priority),
          priorityBadgeClass: getPriorityBadgeClass(t.priority),
          departmentLabel: getDepartmentLabel(t.department),

          createdAtLabel: formatDate(t.createdAt),
        };
      }),
    );

    const currentFilters = computed(() => domainStore.filters());
    const totalCount = computed(() => domainStore.totalCount());
    const pageIndex = computed(() => currentFilters().pageIndex);
    const pageSize = computed(() => currentFilters().pageSize);
    const totalPages = computed(() => Math.max(1, Math.ceil(totalCount() / pageSize())));

    return {
      tickets,
      currentFilters,
      totalCount,
      pageIndex,
      pageSize,
      totalPages,

      startItem: computed(() => (totalCount() === 0 ? 0 : (pageIndex() - 1) * pageSize() + 1)),
      endItem: computed(() => Math.min(pageIndex() * pageSize(), totalCount())),

      visiblePages: computed(() => {
        const total = totalPages();
        const current = pageIndex();
        const range = 2;
        const pages: number[] = [];
        for (let i = Math.max(1, current - range); i <= Math.min(total, current + range); i++) {
          pages.push(i);
        }
        return pages;
      }),

      isLoading: computed(() => domainStore.isLoading()),
      isEmpty: computed(() => domainStore.entities().length === 0 && !domainStore.isLoading()),
    };
  }),

  // متدها فقط Delegation به domainStore هستند
  withMethods((store, domainStore = inject(TicketingStore)) => ({
    // جستجو با debounce تا با هر کلید ریکوئست نرود
    search: rxMethod<string>(
      pipe(
        debounceTime(300),
        distinctUntilChanged(),
        tap((searchString) => domainStore.updateFilters({ searchString, pageIndex: 1 })),
      ),
    ),

    changePage(pageIndex: number) {
      if (pageIndex < 1 || pageIndex > store.totalPages()) return;
      domainStore.updateFilters({ pageIndex });
    },

    filterByDepartment(department: TicketDepartment | null) {
      domainStore.updateFilters({ filterByDepartment: department, pageIndex: 1 });
    },

    filterByStatus(status: TicketStatus | null) {
      domainStore.updateFilters({ filterByStatus: status, pageIndex: 1 });
    },

    toggleSortByPriority() {
      domainStore.updateFilters({
        sortByPriority: !domainStore.filters().sortByPriority,
        pageIndex: 1,
      });
    },

    deleteTicket(ticketId: string) {
      if (confirm('آیا از حذف این تیکت اطمینان دارید؟')) {
        domainStore.deleteTicket(ticketId);
      }
    },
  })),
  withHooks({
    onInit(store, domainStore = inject(TicketingStore)) {
      domainStore.loadTickets(domainStore.filters);
    },
  }),
);
