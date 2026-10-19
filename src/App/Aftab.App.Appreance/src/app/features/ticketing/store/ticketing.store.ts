// ۱. تعریف مدل اختصاصی برای انتیتی‌ها
import {
  AddTicketRequest,
  GetTicketResponse,
  GetTicketsRequest,
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
  addEntity,
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
import { concatMap, delay, exhaustMap, pipe, switchMap, tap } from 'rxjs';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { tapResponse } from '@ngrx/operators';
import { ProblemDetails } from '../../../core/models/errors/problem-details';
import { TicketStatus } from '../enums/ticket-status.enum';

export type TicketEntity = GetTicketResponse & { id: string };

// ۲. متد کمکی برای تبدیل Response به Entity
const mapToEntity = (res: GetTicketResponse): TicketEntity => ({
  id: res.ticket.id,
  ...res,
});

// ۳. تعریف State برای فیلترها (بسیار مهم برای صفحه‌بندی و جستجو)
interface TicketingState {
  filters: GetTicketsRequest;
  selectedTicketId: string | null;
}

const initialState: TicketingState = {
  filters: {
    searchString: '',
    sortByPriority: false,
    filterByPersonId: null,
    filterByStatus: null,
    filterByDepartment: null,
    pageIndex: 1,
    pageSize: 10,
  },
  selectedTicketId: null
};

export const TicketingStore = signalStore(
  { providedIn: 'root' }, // اگر این Store گلوبال است. اگر مربوط به یک پنل خاص است، در کامپوننت پروواید شود.

  withState(initialState),
  withEntities<TicketEntity>(), // جادوی NgRx: اضافه شدن entityMap, ids, entities
  withRequestStatus(), // مدیریت هوشمند Loading و Error که قبلا ساختیم

  withComputed(({ entityMap, selectedTicketId }) => ({
    // این سیگنال اتوماتیک تیکت فعال را برمی‌گرداند.
    // اگر آیدی نبود یا تیکت در لیست نبود، null برمی‌گرداند.
    selectedTicket: computed(() => {
      const id = selectedTicketId();
      return id ? (entityMap()[id] ?? null) : null;
    }),
  })),

  withMethods((store, ticketingService = inject(TicketingService)) => ({
    // --- کوئری‌ها (Queries - GET) ---
    setSelectedTicketId(id: string | null) {
      patchState(store, { selectedTicketId: id });
    },
    // استفاده از switchMap: اگر کاربر تند تند فیلتر را عوض کرد، فقط ریکوئست آخر مهم است
    loadTickets: rxMethod<GetTicketsRequest>(
      pipe(
        tap(() => patchState(store, setPending())),
        switchMap((filters) =>
          ticketingService.getTickets(filters).pipe(
            tapResponse({
              next: (responses) => {
                const entities = responses.map(mapToEntity);
                // setAllEntities دیکشنری قبلی را پاک کرده و با دیتای جدید جایگزین می‌کند
                patchState(store, setAllEntities(entities), setFulfilled());
              },
              error: (err: ProblemDetails) => patchState(store, setRejected(err)),
            }),
          ),
        ),
      ),
    ),

    // بارگذاری یک تیکت خاص (مثلا برای رفرش کردن دیتا بعد از افزودن ریپلای)
    loadSingleTicket: rxMethod<string>(
      pipe(
        switchMap((ticketId) =>
          ticketingService.getTicket(ticketId).pipe(
            tapResponse({
              next: (response) => {
                console.log(response);
                const entity = mapToEntity(response);
                // addEntity یا updateEntity به صورت هوشمند عمل می‌کنند.
                // ما از setEntity استفاده می‌کنیم که اگر بود آپدیت کند، اگر نبود بسازد.
                patchState(
                  store,
                  setEntity(entity), // ✅ جایگزین شد
                  { selectedTicketId: entity.id },
                  setFulfilled(),
                );
              },
              error: (err: ProblemDetails) => console.error(err), // هندلینگ خطای سایلنت
            }),
          ),
        ),
      ),
    ),
  })),

  withMethods((store, ticketingService = inject(TicketingService)) => ({
    // --- مدیریت فیلترها ---
    updateFilters(newFilters: Partial<GetTicketsRequest>) {
      patchState(store, (state) => ({
        filters: { ...state.filters, ...newFilters },
      }));
    },

    // --- کامندها (Commands - POST / PATCH / DELETE) ---

    // استفاده از concatMap: اگر کاربر دو بار پشت سر هم وضعیت را تغییر داد، به ترتیب اجرا شود
    // changeStatus: rxMethod<{ id: string; status: TicketStatus }>(
    //   pipe(
    //     tap(() => patchState(store, setPending())),
    //     concatMap(({ id, status }) =>
    //       ticketingService.changeStatus(id, status).pipe(
    //         tapResponse({
    //           next: () => {
    //             // آپدیت کردن Entity در کش کلاینت (بدون نیاز به ریکوئست مجدد)
    //             patchState(
    //               store,
    //               updateEntity({
    //                 id,
    //                 changes: (entity) => ({
    //                   data: { ...entity.ticket, status },
    //                 }),
    //               }),
    //               setFulfilled(),
    //             );
    //           },
    //           error: (err: ProblemDetails) => patchState(store, setRejected(err)),
    //         }),
    //       ),
    //     ),
    //   ),
    // ),

    addTicket: rxMethod<AddTicketRequest>(
      pipe(
        tap(() => patchState(store, setPending())),
        exhaustMap((req) =>
          ticketingService.AddTicket(req).pipe(
            tapResponse({
              next: () => {
                patchState(store, setFulfilled());
              },
              error: (err: ProblemDetails) => {
                patchState(store, setRejected(err));
              },
            }),
          ),
        ),
      ),
    ),

    // استفاده از exhaustMap: جلوگیری از دبل کلیک کاربر روی دکمه حذف
    deleteTicket: rxMethod<string>(
      pipe(
        tap(() => patchState(store, setPending())),
        exhaustMap((id) =>
          ticketingService.softDelete(id).pipe(
            tapResponse({
              next: () => {
                // حذف بلافاصله تیکت از UI پس از تایید سرور
                patchState(store, removeEntity(id), setFulfilled());
              },
              error: (err: ProblemDetails) => patchState(store, setRejected(err)),
            }),
          ),
        ),
      ),
    ),

    // عملیات‌هایی که void برمی‌گردانند و ساختار پیچیده (لیست Replies) دارند
    // منطقی‌ترین کار: بعد از موفقیت، کل آن تیکت را مجدد از سرور بگیریم
    addReply: rxMethod<{ content: string; status: TicketStatus }>(
      pipe(
        tap(() => patchState(store, setPending())),
        exhaustMap(({ content, status }) =>
          ticketingService.addReply(store.selectedTicketId()!, content, status).pipe(
            tapResponse({
              next: () => {
                patchState(store, setFulfilled());
                // رفرش کردن تیکت برای دریافت دیتای جدید به همراه آیدی ریپلای ایجاد شده
                // به این تکنیک Dispatch Action Delegation می‌گویند
                delay(5000)
                store.loadSingleTicket(store.selectedTicketId()!);
              },
              error: (err: ProblemDetails) => patchState(store, setRejected(err)),
            }),
          ),
        ),
      ),
    ),

    deleteReply: rxMethod<{ ticketId: string; replyId: string }>(
      pipe(
        tap(() => patchState(store, setPending())),
        exhaustMap(({ ticketId, replyId }) =>
          ticketingService.softDeleteReply(ticketId, replyId).pipe(
            tapResponse({
              next: () => {
                patchState(store, setFulfilled());
                // آپدیت دستی کلاینت ساید (چون حذف نیازی به رفرش از سرور ندارد و آیدی را داریم)
                patchState(
                  store,
                  updateEntity({
                    id: ticketId,
                    changes: (entity) => ({
                      replies: entity.replies.filter((r) => r.id !== replyId),
                    }),
                  }),
                );
              },
              error: (err: ProblemDetails) => patchState(store, setRejected(err)),
            }),
          ),
        ),
      ),
    ),
  })),

  // --- شاهکار معماری Reactive: گره زدن فیلترها به متد Load ---
  withHooks({
    onInit(store) {
      // به محض اینکه store مقداردهی شد، یا هر زمان که سیگنال store.filters تغییر کرد،
      // به صورت اتوماتیک متد loadTickets کال می‌شود!
      store.loadTickets(store.filters);
    },
  }),
);
