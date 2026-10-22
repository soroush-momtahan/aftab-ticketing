import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { form, FormField, maxLength, minLength, required } from '@angular/forms/signals';
import { NewReply, NewReplyInput } from './components/new-reply/new-reply';
import { ReplyVm, TicketDetailStore } from './store/ticket-detail.store';

@Component({
  selector: 'app-ticket-detail',
  imports: [RouterLink, FormField, NewReply, NewReply],
  providers: [TicketDetailStore],
  host: { '(document:keydown.escape)': 'closeDialogs()' },
  styleUrl: './ticket-detail.css',
  templateUrl: './ticket-detail.html',
})
export class TicketDetail {
  protected readonly store = inject(TicketDetailStore);

  // ---------- Edit dialog ----------
  protected readonly editing = signal<ReplyVm | null>(null);
  protected readonly visibilityIconSwitcher = signal(false);
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
  protected onUpdateReplyVisibility(replyId: string, visibility: boolean) {
    this.store.updateReplyVisibility(replyId, !visibility);
  }
}
