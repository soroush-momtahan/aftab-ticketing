// import { signalStore, withComputed, withMethods } from '@ngrx/signals';
// import { computed, inject } from '@angular/core';
// import { GetTicketResponse, ReplyTicketDto, TicketDto } from '../../../services/ticketing.service';
// import { TicketStatus } from '../../../enums/ticket-status.enum';
// import { TicketPriority } from '../../../enums/ticket-priority.enum';
// import { TicketDepartment } from '../../../enums/ticket-department.enum';
// import { TicketEntity, TicketingStore } from '../../../store/ticketing.store';
//
// export interface TicketDetailViewModel extends GetTicketResponse {
//   statusLabel: string;
//   statusBadgeClass: string;
//   statusDotClass: string;
//   departmentLabel: string;
//   priorityLabel: string;
//   priorityBadgeClass: string;
//   createdAtLabel: string;
// }
//
// export const TicketDetailStore = signalStore(
//   withComputed((store, domainStore = inject(TicketingStore)) => {
//     const activeEntity = computed<TicketEntity | null>(() => domainStore.selectedTicket());
//     return {
//       ticketInfo: computed<TicketDto | null>(() => activeEntity()?.ticket ?? null),
//       replies: computed<ReplyTicketDto[]>(() => activeEntity()?.replies ?? []),
//       statusLabel: computed(() => {
//         const t = activeEntity();
//         if (!t) return '';
//         const map: Record<TicketStatus, string> = {
//           1: 'باز',
//           2: 'در حال بررسی',
//           3: 'حل شده',
//           4: 'بسته',
//         };
//         return map[t.ticket.status];
//       }),
//       statusBadgeClass: computed(() => {
//         const t = activeEntity();
//         if (!t) return '';
//         const map: Record<TicketStatus, string> = {
//           1: 'border-[#e3ab1c]/40 bg-[#e3ab1c]/10 text-[#e3ab1c]',
//           2: 'border-blue-400/40 bg-blue-400/10 text-blue-300',
//           3: 'border-emerald-400/40 bg-emerald-400/10 text-emerald-300',
//           4: 'border-[#3a3a3a] bg-[#1a1a1a] text-[#888]',
//         };
//         return map[t.ticket.status];
//       }),
//       statusDotClass: computed(() => {
//         const t = activeEntity();
//         if (!t) return '';
//         const map: Record<TicketStatus, string> = {
//           1: 'bg-[#e3ab1c]',
//           2: 'bg-blue-400',
//           3: 'bg-emerald-400',
//           4: 'bg-[#555]',
//         };
//         return map[t.ticket!.status];
//       }),
//       departmentLabel: computed(() => {
//         const t = activeEntity();
//         if (!t) return '';
//         const map: Record<TicketDepartment, string> = {
//           1: 'پشتیبانی',
//           2: 'فروش',
//           3: 'فنی',
//           4: 'فروش',
//         };
//         return map[t.ticket.department];
//       }),
//       priorityLabel: computed(() => {
//         const t = activeEntity();
//         if (!t) return '';
//         const map: Record<TicketPriority, string> = {
//           1: 'کم',
//           2: 'متوسط',
//           3: 'زیاد',
//         };
//         return map[t.ticket.priority];
//       }),
//       priorityClass: computed(() => {
//         const t = activeEntity();
//         if (!t) return '';
//         const map: Record<TicketPriority, string> = {
//           1: 'text-[#a3a3a3]',
//           2: 'text-[#e9bb49]',
//           3: 'text-[#e3ab1c]',
//         };
//         return map[t.ticket.priority];
//       }),
//       isSubmitting: computed(() => domainStore.isLoading()),
//       isSuccess: computed(() => domainStore.isSuccess()),
//       hasError: computed(() => domainStore.hasError()),
//     };
//   }),
//   withMethods((store, domainStore = inject(TicketingStore)) => ({
//     addReply(content: string, status: TicketStatus) {
//       domainStore.addReply({ content, status });
//     },
//     getTicket(ticketId: string) {
//       domainStore.loadSingleTicket(ticketId);
//     },
//   })),
// );

// مسیر: ticketing/pages/ticket-detail/store/ticket-detail.store.ts
import { computed, effect, inject, untracked } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute } from '@angular/router';
import { map } from 'rxjs';
import { signalStore, withComputed, withHooks, withMethods } from '@ngrx/signals';
import { ReplyTicketDto, TicketDto } from '../../../services/ticketing.service';
import { TicketStatus } from '../../../enums/ticket-status.enum';
import { TicketingStore } from '../../../store/ticketing.store';
import {
  formatDate,
  getDepartmentLabel,
  getPriorityLabel,
  getPriorityTextClass,
  getStatusBadgeClass,
  getStatusDotClass,
  getStatusLabel,
} from '../../../mappers/ticket.mappers';
import { TICKETING_USER } from '../../../token/ticketing-user.token';

export interface TicketDetailVm extends TicketDto {
  initial: string;
  createdAtLabel: string;
  modifiedAtLabel: string;
  statusLabel: string;
  statusBadgeClass: string;
  statusDotClass: string;
  departmentLabel: string;
  priorityLabel: string;
  priorityTextClass: string;
}

export interface ReplyVm extends ReplyTicketDto {
  initial: string;
  canEdit: boolean;
  canDelete: boolean;
}

export const TicketDetailStore = signalStore(
  withComputed((_, domain = inject(TicketingStore), user = inject(TICKETING_USER)) => {
    const entity = domain.selectedTicket;

    const ticket = computed<TicketDetailVm | null>(() => {
      const t = entity()?.ticket;
      if (!t) return null;
      return {
        ...t,
        initial: t.displayName.charAt(0),
        createdAtLabel: formatDate(t.createdAt),
        modifiedAtLabel: formatDate(t.modifiedAt),
        statusLabel: getStatusLabel(t.status),
        statusBadgeClass: getStatusBadgeClass(t.status),
        statusDotClass: getStatusDotClass(t.status),
        departmentLabel: getDepartmentLabel(t.department),
        priorityLabel: getPriorityLabel(t.priority),
        priorityTextClass: getPriorityTextClass(t.priority),
      };
    });

    const replies = computed<ReplyVm[]>(() => {
      const { id, isAdmin } = user();
      return (entity()?.replies ?? []).map((r) => {
        const isOwn = id !== null && r.replierId === id;
        return {
          ...r,
          initial: r.replierDisplayName.charAt(0),
          canEdit: isOwn,
          canDelete: isOwn || isAdmin,
        };
      });
    });

    return {
      ticket,
      replies,
      isAdmin: computed(() => user().isAdmin),
      isLoading: domain.isLoading,
      hasError: domain.hasError,
      replyState: domain.replyState,
    };
  }),

  // فقط Delegation به domain store
  withMethods((_, domain = inject(TicketingStore)) => ({
    addReply: (content: string, status: TicketStatus) => domain.addReply({ content, status }),
    editReply: (replyId: string, content: string) =>
      domain.updateReply({ ticketId: domain.selectedTicketId()!, replyId, content }),
    removeReply: (replyId: string) =>
      domain.deleteReply({ ticketId: domain.selectedTicketId()!, replyId }),
  })),

  // آیدی از روت خوانده می‌شود؛ با تغییر پارامتر (رفتن از یک تیکت به تیکت دیگر) هم دوباره لود می‌شود
  withHooks(() => {
    const domain = inject(TicketingStore);
    const ticketId = toSignal(inject(ActivatedRoute).paramMap.pipe(map((p) => p.get('id'))), {
      initialValue: null,
    });

    return {
      onInit() {
        effect(() => {
          const id = ticketId();
          if (id) untracked(() => domain.loadSingleTicket(id));
        });
      },
      onDestroy() {
        domain.setSelectedTicketId(null);
      },
    };
  }),
);
