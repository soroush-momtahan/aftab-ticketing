import { Component, computed, inject, input, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TicketDetailStore } from './store/ticket-detail.store';
import { form, FormField, maxLength, minLength, required } from '@angular/forms/signals';
import { NewReply, NewReplyInput } from './components/new-reply/new-reply';
import { TicketingStore } from '../../store/ticketing.store';
import { tick } from '@angular/core/testing';
import { ActivatedRoute } from '@angular/router';

type TicketStatus = 'open' | 'in_progress' | 'resolved' | 'closed';
type Department = 'support' | 'sales' | 'technical';
type Priority = 'low' | 'medium' | 'high';

interface Ticket {
  id: string;
  title: string;
  description: string;
  status: TicketStatus;
  department: Department;
  priority: Priority;
  createdAt: string;
  updatedAt: string;
  customerName: string;
}

interface Reply {
  id: string;
  senderName: string;
  isStaff: boolean;
  content: string;
  createdAt: string;
  editedAt: string | null;
  isOwn: boolean; // for showing edit/delete controls
}

interface TicketDetailInputModel {
  editedTicketContent: string;
  newTicketStatus: TicketStatus | null;
  newTicketSReply: string;
}
@Component({
  imports: [FormsModule, NewReply],
  providers: [TicketDetailStore, TicketingStore],
  selector: 'app-ticket-detail',
  styleUrl: './ticket-detail.css',
  templateUrl: './ticket-detail.html',
})
export class TicketDetail implements OnInit {
  private route = inject(ActivatedRoute);
  constructor() {}
  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id){
      this.store.getTicket(id);
    }
  }
  readonly store = inject(TicketDetailStore);

  // ========== UI STATE ==========
  readonly newReplyText = signal('');
  readonly isSubmitting = signal(false);

  readonly editingReply = signal<Reply | null>(null);
  readonly editContent = signal('');

  readonly deletingReplyId = signal<string | null>(null);

  // ========== ACTIONS ==========
  onReplySentEvent(replyInput: NewReplyInput): void {
    this.isSubmitting.set(true);
    this.store.addReply(replyInput.content, replyInput.status!);
  }

  openEdit(reply: Reply) {
    this.editingReply.set(reply);
    this.editContent.set(reply.content);
  }

  closeEdit() {
    this.editingReply.set(null);
    this.editContent.set('');
  }

  saveEdit() {
    // const content = this.editContent().trim();
    // const target = this.editingReply();
    // if (!content || !target) return;
    //
    // this.replies.update((list) =>
    //   list.map((r) => (r.id === target.id ? { ...r, content, editedAt: this.getNow() } : r)),
    // );
    // this.closeEdit();
  }

  openDelete(id: string) {
    this.deletingReplyId.set(id);
  }

  closeDelete() {
    this.deletingReplyId.set(null);
  }

  confirmDelete() {
    // const id = this.deletingReplyId();
    // if (!id) return;
    // this.replies.update((list) => list.filter((r) => r.id !== id));
    // this.closeDelete();
  }

  private getNow(): string {
    // Simple Persian-style timestamp for demo
    const now = new Date();
    return `۱۴۰۳/۰۷/۱۳ - ${now.getHours().toString().padStart(2, '0')}:${now
      .getMinutes()
      .toString()
      .padStart(2, '0')}`;
  }

  protected readonly tick = tick;
}
