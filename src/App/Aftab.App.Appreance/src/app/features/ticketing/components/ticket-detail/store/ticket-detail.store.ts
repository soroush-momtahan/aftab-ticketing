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
  canChangeVisibility: boolean;
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
          canEdit: isOwn || isAdmin,
          canDelete: isOwn || isAdmin,
          canChangeVisibility: isAdmin,
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
    updateReplyVisibility: (replyTicketId: string, visibility: boolean) =>
      domain.editReplyVisibility({ ticketId: domain.selectedTicketId()!, replyTicketId, visibility }),
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
