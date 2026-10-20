// import { Component, inject, OnInit, signal } from '@angular/core';
// import { FormsModule } from '@angular/forms';
// import { TicketDetailStore } from './store/ticket-detail.store';
// import { NewReply, NewReplyInput } from './components/new-reply/new-reply';
// import { TicketingStore } from '../../store/ticketing.store';
// import { tick } from '@angular/core/testing';
// import { ActivatedRoute } from '@angular/router';
//
//
// interface Reply {
//   id: string;
//   senderName: string;
//   isStaff: boolean;
//   content: string;
//   createdAt: string;
//   editedAt: string | null;
//   isOwn: boolean; // for showing edit/delete controls
// }
// @Component({
//   imports: [FormsModule, NewReply],
//   providers: [TicketDetailStore, TicketingStore],
//   selector: 'app-ticket-detail',
//   styleUrl: './ticket-detail.css',
//   templateUrl: './ticket-detail.html',
// })
// export class TicketDetail implements OnInit {
//   private route = inject(ActivatedRoute);
//   constructor() {}
//   ngOnInit(): void {
//     const id = this.route.snapshot.paramMap.get('id');
//     if (id){
//       this.store.getTicket(id);
//     }
//   }
//   readonly store = inject(TicketDetailStore);
//
//   // ========== UI STATE ==========
//   readonly newReplyText = signal('');
//   readonly isSubmitting = signal(false);
//
//   readonly editingReply = signal<Reply | null>(null);
//   readonly editContent = signal('');
//
//   readonly deletingReplyId = signal<string | null>(null);
//
//   // ========== ACTIONS ==========
//   onReplySentEvent(replyInput: NewReplyInput): void {
//     this.isSubmitting.set(true);
//     this.store.addReply(replyInput.content, replyInput.status!);
//   }
//
//   openEdit(reply: Reply) {
//     this.editingReply.set(reply);
//     this.editContent.set(reply.content);
//   }
//
//   closeEdit() {
//     this.editingReply.set(null);
//     this.editContent.set('');
//   }
//
//   saveEdit() {
//     // const content = this.editContent().trim();
//     // const target = this.editingReply();
//     // if (!content || !target) return;
//     //
//     // this.replies.update((list) =>
//     //   list.map((r) => (r.id === target.id ? { ...r, content, editedAt: this.getNow() } : r)),
//     // );
//     // this.closeEdit();
//   }
//
//   openDelete(id: string) {
//     this.deletingReplyId.set(id);
//   }
//
//   closeDelete() {
//     this.deletingReplyId.set(null);
//   }
//
//   confirmDelete() {
//     // const id = this.deletingReplyId();
//     // if (!id) return;
//     // this.replies.update((list) => list.filter((r) => r.id !== id));
//     // this.closeDelete();
//   }
//
//   private getNow(): string {
//     // Simple Persian-style timestamp for demo
//     const now = new Date();
//     return `۱۴۰۳/۰۷/۱۳ - ${now.getHours().toString().padStart(2, '0')}:${now
//       .getMinutes()
//       .toString()
//       .padStart(2, '0')}`;
//   }
//
//   protected readonly tick = tick;
// }


// مسیر: ticketing/pages/ticket-detail/ticket-detail.ts
import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { form, FormField, maxLength, minLength, required } from '@angular/forms/signals';
import { NewReply, NewReplyInput } from './components/new-reply/new-reply';
import { ReplyVm, TicketDetailStore } from './store/ticket-detail.store';

@Component({
  selector: 'app-ticket-detail',
  imports: [RouterLink, FormField, NewReply, NewReply],
  providers: [TicketDetailStore], // TicketingStore گلوبال است و دیگر اینجا پروواید نمی‌شود
  host: { '(document:keydown.escape)': 'closeDialogs()' },
  styleUrl: './ticket-detail.css',
  templateUrl: './ticket-detail.html',
})
export class TicketDetail {
  protected readonly store = inject(TicketDetailStore);

  // ---------- Edit dialog ----------
  protected readonly editing = signal<ReplyVm | null>(null);
  protected readonly editModel = signal({ content: '' });
  protected readonly editForm = form(this.editModel, (f) => {
    required(f.content, { message: 'متن پاسخ نمی‌تواند خالی باشد' });
    minLength(f.content, 6, { message: 'متن پاسخ خیلی کوتاه است' });
    maxLength(f.content, 3000, { message: 'متن پاسخ خیلی طولانی است' });
  });

  // ---------- Delete dialog ----------
  protected readonly deletingId = signal<string | null>(null);

  protected onReplySent({ content, status }: NewReplyInput) {
    this.store.addReply(content, status);
  }

  protected openEdit(reply: ReplyVm) {
    this.editing.set(reply);
    this.editModel.set({ content: reply.content });
  }

  protected saveEdit() {
    const target = this.editing();
    if (!target || this.editForm().invalid()) return;

    const content = this.editModel().content.trim();
    if (content !== target.content) this.store.editReply(target.id, content);
    this.editing.set(null);
  }

  protected confirmDelete() {
    const id = this.deletingId();
    if (!id) return;
    this.store.removeReply(id);
    this.deletingId.set(null);
  }

  protected closeDialogs() {
    this.editing.set(null);
    this.deletingId.set(null);
  }
}
