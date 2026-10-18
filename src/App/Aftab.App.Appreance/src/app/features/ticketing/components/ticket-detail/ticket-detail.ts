import { Component, computed, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';


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
@Component({
  imports: [FormsModule],
  selector: 'app-ticket-detail',
  styleUrl: './ticket-detail.css',
  templateUrl: './ticket-detail.html',
})
export class TicketDetail {
  // ========== MOCK DATA (replace with real API later) ==========
  readonly ticket = signal<Ticket>({
    id: 'TK-24087',
    title: 'صدای لنت جلو بعد از تعویض',
    description:
      'سلام، لنت‌های سرامیکی جلو رو حدود ۳۰۰۰ کیلومتر پیش تعویض کردم. از هفته پیش موقع ترمزگیری صدای سوت بلندی می‌ده و احساس می‌کنم قدرت ترمز کمی کاهش پیدا کرده. ماشین BMW سری ۵ مدل ۲۰۲۱ هست. لطفاً راهنمایی کنید.',
    status: 'in_progress',
    department: 'technical',
    priority: 'high',
    createdAt: '۱۴۰۳/۰۷/۱۲ - ۱۴:۳۲',
    updatedAt: '۱۴۰۳/۰۷/۱۳ - ۰۹:۱۵',
    customerName: 'علی رضایی',
  });

  readonly replies = signal<Reply[]>([
    {
      id: 'r1',
      senderName: 'سارا محمدی',
      isStaff: true,
      content:
        'سلام آقای رضایی، از پیام شما متشکریم.\nلطفاً بفرمایید آیا بعد از تعویض، لنت‌ها به درستی bedding-in شده‌اند؟ همچنین اگر امکان دارد عکس از سطح لنت و دیسک ارسال کنید.',
      createdAt: '۱۴۰۳/۰۷/۱۲ - ۱۶:۴۵',
      editedAt: null,
      isOwn: false,
    },
    {
      id: 'r2',
      senderName: 'علی رضایی',
      isStaff: false,
      content:
        'بله، طبق دستورالعمل bedding انجام دادم. عکس‌ها رو هم همین الان آپلود می‌کنم. ممنون از پیگیری سریع.',
      createdAt: '۱۴۰۳/۰۷/۱۲ - ۱۸:۱۰',
      editedAt: '۱۴۰۳/۰۷/۱۲ - ۱۸:۲۲',
      isOwn: true,
    },
  ]);

  // ========== UI STATE ==========
  readonly newReplyText = signal('');
  readonly isSubmitting = signal(false);

  readonly editingReply = signal<Reply | null>(null);
  readonly editContent = signal('');

  readonly deletingReplyId = signal<string | null>(null);

  readonly isAdmin = signal(true); // ← set to false for normal customers
  readonly selectedStatus = signal<TicketStatus | null>(null);

  // Status options for the segmented control
  readonly statusOptions: { value: TicketStatus; label: string }[] = [
    { value: 'open', label: 'باز' },
    { value: 'in_progress', label: 'در حال بررسی' },
    { value: 'resolved', label: 'حل شده' },
    { value: 'closed', label: 'بسته' },
  ];

  // ========== COMPUTED LABELS ==========
  statusLabel = computed(() => {
    const map: Record<TicketStatus, string> = {
      open: 'باز',
      in_progress: 'در حال بررسی',
      resolved: 'حل شده',
      closed: 'بسته',
    };
    return map[this.ticket().status];
  });

  statusBadgeClass = computed(() => {
    const map: Record<TicketStatus, string> = {
      open: 'border-[#e3ab1c]/40 bg-[#e3ab1c]/10 text-[#e3ab1c]',
      in_progress: 'border-blue-400/40 bg-blue-400/10 text-blue-300',
      resolved: 'border-emerald-400/40 bg-emerald-400/10 text-emerald-300',
      closed: 'border-[#3a3a3a] bg-[#1a1a1a] text-[#888]',
    };
    return map[this.ticket().status];
  });

  statusDotClass = computed(() => {
    const map: Record<TicketStatus, string> = {
      open: 'bg-[#e3ab1c]',
      in_progress: 'bg-blue-400',
      resolved: 'bg-emerald-400',
      closed: 'bg-[#555]',
    };
    return map[this.ticket().status];
  });

  departmentLabel = computed(() => {
    const map: Record<Department, string> = {
      support: 'پشتیبانی',
      sales: 'فروش',
      technical: 'فنی',
    };
    return map[this.ticket().department];
  });

  priorityLabel = computed(() => {
    const map: Record<Priority, string> = {
      low: 'کم',
      medium: 'متوسط',
      high: 'زیاد',
    };
    return map[this.ticket().priority];
  });

  priorityClass = computed(() => {
    const map: Record<Priority, string> = {
      low: 'text-[#a3a3a3]',
      medium: 'text-[#e9bb49]',
      high: 'text-[#e3ab1c]',
    };
    return map[this.ticket().priority];
  });

  // ========== ACTIONS ==========
  sendReply() {
    const text = this.newReplyText().trim();
    if (!text || this.isSubmitting()) return;

    this.isSubmitting.set(true);

    // Simulate API delay
    setTimeout(() => {
      const newReply: Reply = {
        id: `r${Date.now()}`,
        senderName: 'علی رضایی',
        isStaff: false,
        content: text,
        createdAt: this.getNow(),
        editedAt: null,
        isOwn: true,
      };
      this.replies.update((list) => [...list, newReply]);
      this.newReplyText.set('');
      this.isSubmitting.set(false);
    }, 600);
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
    const content = this.editContent().trim();
    const target = this.editingReply();
    if (!content || !target) return;

    this.replies.update((list) =>
      list.map((r) => (r.id === target.id ? { ...r, content, editedAt: this.getNow() } : r)),
    );
    this.closeEdit();
  }

  openDelete(id: string) {
    this.deletingReplyId.set(id);
  }

  closeDelete() {
    this.deletingReplyId.set(null);
  }

  confirmDelete() {
    const id = this.deletingReplyId();
    if (!id) return;
    this.replies.update((list) => list.filter((r) => r.id !== id));
    this.closeDelete();
  }

  private getNow(): string {
    // Simple Persian-style timestamp for demo
    const now = new Date();
    return `۱۴۰۳/۰۷/۱۳ - ${now.getHours().toString().padStart(2, '0')}:${now
      .getMinutes()
      .toString()
      .padStart(2, '0')}`;
  }
}
