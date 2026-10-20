// import {
//   AddTicketRequest,
//   GetTicketResponse,
//   GetTicketsRequest,
//   TicketingService,
// } from '../services/ticketing.service';
// import {
//   patchState,
//   signalStore,
//   withComputed,
//   withHooks,
//   withMethods,
//   withState,
// } from '@ngrx/signals';
// import {
//   removeEntity,
//   setAllEntities,
//   setEntity,
//   updateEntity,
//   withEntities,
// } from '@ngrx/signals/entities';
// import {
//   setFulfilled,
//   setPending,
//   setRejected,
//   withRequestStatus,
// } from '../../../core/store/features/request-status.festure';
// import { computed, inject } from '@angular/core';
// import { exhaustMap, pipe, switchMap, tap } from 'rxjs';
// import { rxMethod } from '@ngrx/signals/rxjs-interop';
// import { tapResponse } from '@ngrx/operators';
// import { ProblemDetails } from '../../../core/models/errors/problem-details';
// import { TicketStatus } from '../enums/ticket-status.enum';
// import { TicketsPage } from '../models/tickets-page.model';
//
// export type TicketEntity = GetTicketResponse & { id: string };
//
// const mapToEntity = (res: GetTicketResponse): TicketEntity => ({
//   id: res.ticket.id,
//   ...res,
// });
//
// interface TicketingState {
//   filters: GetTicketsRequest;
//   selectedTicketId: string | null;
//   totalCount: number; // تعداد کل نتایج (قبل از صفحه‌بندی) از سرور
// }
//
// const initialState: TicketingState = {
//   filters: {
//     searchString: '',
//     sortByPriority: false,
//     filterByPersonId: null,
//     filterByStatus: null,
//     filterByDepartment: null,
//     pageIndex: 1,
//     pageSize: 8,
//   },
//   selectedTicketId: null,
//   totalCount: 0,
// };
//
// export const TicketingStore = signalStore(
//   { providedIn: 'root' },
//
//   withState(initialState),
//   withEntities<TicketEntity>(),
//   withRequestStatus(),
//
//   withComputed(({ entityMap, selectedTicketId }) => ({
//     selectedTicket: computed(() => {
//       const id = selectedTicketId();
//       return id ? (entityMap()[id] ?? null) : null;
//     }),
//   })),
//
//   withMethods((store, ticketingService = inject(TicketingService)) => ({
//     setSelectedTicketId(id: string | null) {
//       patchState(store, { selectedTicketId: id });
//     },
//     loadTickets: rxMethod<GetTicketsRequest>(
//       pipe(
//         tap(() => patchState(store, setPending())),
//         switchMap((filters) =>
//           ticketingService.getTickets(filters).pipe(
//             tapResponse({
//               next: (page: TicketsPage) => {
//                 patchState(
//                   store,
//                   setAllEntities(page.data.map(mapToEntity)),
//                   { totalCount: page.count },
//                   setFulfilled(),
//                 );
//               },
//               error: (err: ProblemDetails) => patchState(store, setRejected(err)),
//             }),
//           ),
//         ),
//       ),
//     ),
//
//     loadSingleTicket: rxMethod<string>(
//       pipe(
//         tap(() => patchState(store, setPending())),
//         switchMap((ticketId) =>
//           ticketingService.getTicket(ticketId).pipe(
//             tapResponse({
//               next: (response) => {
//                 const entity = mapToEntity(response);
//                 patchState(store, setEntity(entity), { selectedTicketId: entity.id }, setFulfilled());
//               },
//               error: (err: ProblemDetails) => patchState(store, setRejected(err)),
//             }),
//           ),
//         ),
//       ),
//     ),
//   })),
//
//   withMethods((store, ticketingService = inject(TicketingService)) => ({
//     updateFilters(newFilters: Partial<GetTicketsRequest>) {
//       patchState(store, (state) => ({
//         filters: { ...state.filters, ...newFilters },
//       }));
//     },
//
//     reloadTickets() {
//       store.loadTickets(store.filters());
//     },
//
//     addTicket: rxMethod<AddTicketRequest>(
//       pipe(
//         tap(() => patchState(store, setPending())),
//         exhaustMap((req) =>
//           ticketingService.AddTicket(req).pipe(
//             tapResponse({
//               next: () => {
//                 patchState(store, setFulfilled());
//                 store.loadTickets(store.filters());
//               },
//               error: (err: ProblemDetails) => patchState(store, setRejected(err)),
//             }),
//           ),
//         ),
//       ),
//     ),
//
//     deleteTicket: rxMethod<string>(
//       pipe(
//         tap(() => patchState(store, setPending())),
//         exhaustMap((id) =>
//           ticketingService.softDelete(id).pipe(
//             tapResponse({
//               next: () => {
//                 patchState(store, removeEntity(id), setFulfilled());
//                 // برای اینکه صفحه‌بندی و totalCount دقیق بماند، لیست دوباره گرفته می‌شود
//                 store.loadTickets(store.filters());
//               },
//               error: (err: ProblemDetails) => patchState(store, setRejected(err)),
//             }),
//           ),
//         ),
//       ),
//     ),
//
//     addReply: rxMethod<{ content: string; status: TicketStatus }>(
//       pipe(
//         tap(() => patchState(store, setPending())),
//         exhaustMap(({ content, status }) =>
//           ticketingService.addReply(store.selectedTicketId()!, content, status).pipe(
//             tapResponse({
//               next: () => {
//                 patchState(store, setFulfilled());
//                 // (delay(5000) قبلی هیچ اثری نداشت؛ حذف شد)
//                 store.loadSingleTicket(store.selectedTicketId()!);
//               },
//               error: (err: ProblemDetails) => patchState(store, setRejected(err)),
//             }),
//           ),
//         ),
//       ),
//     ),
//
//     deleteReply: rxMethod<{ ticketId: string; replyId: string }>(
//       pipe(
//         tap(() => patchState(store, setPending())),
//         exhaustMap(({ ticketId, replyId }) =>
//           ticketingService.softDeleteReply(ticketId, replyId).pipe(
//             tapResponse({
//               next: () => {
//                 patchState(
//                   store,
//                   updateEntity({
//                     id: ticketId,
//                     changes: (entity) => ({
//                       replies: entity.replies.filter((r) => r.id !== replyId),
//                     }),
//                   }),
//                   setFulfilled(),
//                 );
//               },
//               error: (err: ProblemDetails) => patchState(store, setRejected(err)),
//             }),
//           ),
//         ),
//       ),
//     ),
//   })),
//
//   // هر بار filters تغییر کند، loadTickets خودکار اجرا می‌شود
//   withHooks({
//     onInit(store) {
//       store.loadTickets(store.filters);
//     },
//   }),
// );


// مسیر: ticketing/store/ticketing.store.ts
import {
  AddTicketRequest,
  GetTicketResponse,
  GetTicketsRequest,
  ReplyTicketDto,
  TicketingService,
} from '../services/ticketing.service';
import {
  patchState,
  signalStore,
  withComputed,
  withHooks,
  withMethods,
  withState,
} from '@ngrx/signals';
import {
  removeEntity,
  setAllEntities,
  setEntity,
  updateEntity,
  withEntities,
} from '@ngrx/signals/entities';
import {
  setFulfilled,
  setPending,
  setRejected,
  withRequestStatus,
} from '../../../core/store/features/request-status.festure';
import { computed, inject } from '@angular/core';
import { catchError, concatMap, exhaustMap, pipe, retry, switchMap, tap } from 'rxjs';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { tapResponse } from '@ngrx/operators';
import { ProblemDetails } from '../../../core/models/errors/problem-details';
import { TicketStatus } from '../enums/ticket-status.enum';
import { TicketsPage } from '../models/tickets-page.model';

export type TicketEntity = GetTicketResponse & { id: string };
export type SubmitState = 'idle' | 'pending' | 'success' | 'error';

const mapToEntity = (res: GetTicketResponse): TicketEntity => ({ id: res.ticket.id, ...res });

// آپدیت immutable لیست پاسخ‌های یک تیکت
const patchReplies = (id: string, fn: (replies: ReplyTicketDto[]) => ReplyTicketDto[]) =>
  updateEntity<TicketEntity>({ id, changes: (e) => ({ replies: fn(e.replies) }) });

interface TicketingState {
  filters: GetTicketsRequest;
  selectedTicketId: string | null;
  totalCount: number;
  replyState: SubmitState; // وضعیت ثبت پاسخ جدید (برای دکمه ارسال و پاک شدن فرم)
}

const initialState: TicketingState = {
  filters: {
    searchString: '',
    sortByPriority: false,
    filterByPersonId: null,
    filterByStatus: null,
    filterByDepartment: null,
    pageIndex: 1,
    pageSize: 8,
  },
  selectedTicketId: null,
  totalCount: 0,
  replyState: 'idle',
};

export const TicketingStore = signalStore(
  { providedIn: 'root' },

  withState(initialState),
  withEntities<TicketEntity>(),
  withRequestStatus(),

  withComputed(({ entityMap, selectedTicketId }) => ({
    selectedTicket: computed(() => {
      const id = selectedTicketId();
      return id ? (entityMap()[id] ?? null) : null;
    }),
  })),

  withMethods((store, ticketingService = inject(TicketingService)) => ({
    setSelectedTicketId(id: string | null) {
      patchState(store, { selectedTicketId: id, replyState: 'idle' });
    },

    loadTickets: rxMethod<GetTicketsRequest>(
      pipe(
        tap(() => patchState(store, setPending())),
        switchMap((filters) =>
          ticketingService.getTickets(filters).pipe(
            tapResponse({
              next: (page: TicketsPage) =>
                patchState(
                  store,
                  setAllEntities(page.data.map(mapToEntity)),
                  { totalCount: page.count },
                  setFulfilled(),
                ),
              error: (err: ProblemDetails) => patchState(store, setRejected(err)),
            }),
          ),
        ),
      ),
    ),

    loadSingleTicket: rxMethod<string>(
      pipe(
        // اگر تیکت قبلا کش شده باشد همان لحظه نمایش داده می‌شود و بعد آپدیت می‌شود
        tap((id) => patchState(store, { selectedTicketId: id, replyState: 'idle' }, setPending())),
        switchMap((id) =>
          ticketingService.getTicket(id).pipe(
            tapResponse({
              next: (res) => patchState(store, setEntity(mapToEntity(res)), setFulfilled()),
              error: (err: ProblemDetails) => patchState(store, setRejected(err)),
            }),
          ),
        ),
      ),
    ),
  })),

  withMethods((store, ticketingService = inject(TicketingService)) => ({
    updateFilters(newFilters: Partial<GetTicketsRequest>) {
      patchState(store, (state) => ({ filters: { ...state.filters, ...newFilters } }));
    },

    reloadTickets() {
      store.loadTickets(store.filters());
    },

    addTicket: rxMethod<AddTicketRequest>(
      pipe(
        tap(() => patchState(store, setPending())),
        exhaustMap((req) =>
          ticketingService.AddTicket(req).pipe(
            tapResponse({
              next: () => {
                patchState(store, setFulfilled());
                store.loadTickets(store.filters());
              },
              error: (err: ProblemDetails) => patchState(store, setRejected(err)),
            }),
          ),
        ),
      ),
    ),

    deleteTicket: rxMethod<string>(
      pipe(
        tap(() => patchState(store, setPending())),
        exhaustMap((id) =>
          ticketingService.softDelete(id).pipe(
            tapResponse({
              next: () => {
                patchState(store, removeEntity(id), setFulfilled());
                store.loadTickets(store.filters());
              },
              error: (err: ProblemDetails) => patchState(store, setRejected(err)),
            }),
          ),
        ),
      ),
    ),

    // ثبت پاسخ: چون سرور چیزی برنمی‌گرداند، بعد از موفقیت تیکت را دوباره می‌گیریم.
    // اگر سرور هنوز پاسخ جدید را نشان نداد (تاخیر read-side/کش)، چند بار با فاصله کوتاه تکرار می‌کنیم
    // تا پاسخ ظاهر شود؛ نه delay ثابت، نه رفرش دستی صفحه.
    addReply: rxMethod<{ content: string; status: TicketStatus }>(
      pipe(
        tap(() => patchState(store, { replyState: 'pending' })),
        exhaustMap(({ content, status }) => {
          const ticketId = store.selectedTicketId()!;
          const expectedCount = (store.selectedTicket()?.replies.length ?? 0) + 1;

          return ticketingService.addReply(ticketId, content, status).pipe(
            switchMap(() =>
              ticketingService.getTicket(ticketId).pipe(
                tap((res) => {
                  if (res.replies.length < expectedCount) throw new Error('stale');
                }),
                retry({ count: 4, delay: 750 }),
                // اگر بعد از چند تلاش هم قدیمی بود، آخرین نسخه را نشان بده
                catchError(() => ticketingService.getTicket(ticketId)),
              ),
            ),
            tapResponse({
              next: (res) =>
                patchState(
                  store,
                  setEntity(mapToEntity(res)),
                  { replyState: 'success' },
                  setFulfilled(),
                ),
              error: (err: ProblemDetails) =>
                patchState(store, { replyState: 'error' }, setRejected(err)),
            }),
          );
        }),
      ),
    ),

    // ویرایش پاسخ: Optimistic update + rollback در صورت خطا
    updateReply: rxMethod<{ ticketId: string; replyId: string; content: string }>(
      pipe(
        concatMap(({ ticketId, replyId, content }) => {
          const snapshot = store.entityMap()[ticketId]?.replies ?? [];
          patchState(
            store,
            patchReplies(ticketId, (rs) =>
              rs.map((r) => (r.id === replyId ? { ...r, content } : r)),
            ),
          );
          return ticketingService.updateReplyContent(ticketId, replyId, content).pipe(
            tapResponse({
              next: () => patchState(store, setFulfilled()),
              error: (err: ProblemDetails) =>
                patchState(
                  store,
                  patchReplies(ticketId, () => snapshot),
                  setRejected(err),
                ),
            }),
          );
        }),
      ),
    ),

    // حذف پاسخ: Optimistic update + rollback در صورت خطا
    deleteReply: rxMethod<{ ticketId: string; replyId: string }>(
      pipe(
        concatMap(({ ticketId, replyId }) => {
          const snapshot = store.entityMap()[ticketId]?.replies ?? [];
          patchState(
            store,
            patchReplies(ticketId, (rs) => rs.filter((r) => r.id !== replyId)),
          );
          return ticketingService.softDeleteReply(ticketId, replyId).pipe(
            tapResponse({
              next: () => patchState(store, setFulfilled()),
              error: (err: ProblemDetails) =>
                patchState(
                  store,
                  patchReplies(ticketId, () => snapshot),
                  setRejected(err),
                ),
            }),
          );
        }),
      ),
    ),
  })),
    // withHooks({
    //   onInit(store) {
    //     store.loadTickets(store.filters);
    //   },
    // }),
  // توجه: هوک لود لیست از اینجا به TicketListStore منتقل شده (توضیحات در پیام).
);
