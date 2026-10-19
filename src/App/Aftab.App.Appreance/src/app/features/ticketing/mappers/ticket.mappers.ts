// مسیر: ticketing/mappers/ticket.mappers.ts
import { TicketStatus } from '../enums/ticket-status.enum';
import { TicketPriority } from '../enums/ticket-priority.enum';
import { TicketDepartment } from '../enums/ticket-department.enum';
import { TicketingStep } from '../enums/ticketing-step.enum';

const STEP_LABELS: Record<TicketingStep, string> = {
  [TicketingStep.ProblemDesc]: 'شرح مشکل',
  [TicketingStep.RelevantSection]: 'بخش',
  [TicketingStep.ProblemPriority]: 'اولویت',
  [TicketingStep.ProblemTitle]: 'عنوان',
  [TicketingStep.Confirmation]: 'تأیید',
};

// ---------- Status ----------
const STATUS_LABELS: Record<TicketStatus, string> = {
  [TicketStatus.Open]: 'باز',
  [TicketStatus.InProgress]: 'در حال بررسی',
  [TicketStatus.Answered]: 'پاسخ داده شده',
  [TicketStatus.Closed]: 'بسته',
};

const STATUS_BADGE_CLASSES: Record<TicketStatus, string> = {
  [TicketStatus.Open]: 'border-[#e3ab1c]/40 bg-[#e3ab1c]/10 text-[#e3ab1c]',
  [TicketStatus.InProgress]: 'border-blue-400/40 bg-blue-400/10 text-blue-300',
  [TicketStatus.Answered]: 'border-emerald-400/40 bg-emerald-400/10 text-emerald-300',
  [TicketStatus.Closed]: 'border-[#3a3a3a] bg-[#1a1a1a] text-[#888]',
};

const STATUS_DOT_CLASSES: Record<TicketStatus, string> = {
  [TicketStatus.Open]: 'bg-[#e3ab1c]',
  [TicketStatus.InProgress]: 'bg-blue-400',
  [TicketStatus.Answered]: 'bg-emerald-400',
  [TicketStatus.Closed]: 'bg-[#555]',
};

export const getStatusLabel = (s: TicketStatus): string => STATUS_LABELS[s] ?? '—';
export const getStatusBadgeClass = (s: TicketStatus): string => STATUS_BADGE_CLASSES[s] ?? '';
export const getStatusDotClass = (s: TicketStatus): string => STATUS_DOT_CLASSES[s] ?? '';

// ---------- Priority ----------
const PRIORITY_LABELS: Record<TicketPriority, string> = {
  [TicketPriority.Low]: 'کم',
  [TicketPriority.Medium]: 'متوسط',
  [TicketPriority.High]: 'زیاد',
};

const PRIORITY_BADGE_CLASSES: Record<TicketPriority, string> = {
  [TicketPriority.Low]: 'border-[#2a2a2a] text-[#888]',
  [TicketPriority.Medium]: 'border-[#e9bb49]/40 text-[#e9bb49]',
  [TicketPriority.High]: 'border-[#e3ab1c]/50 bg-[#e3ab1c]/10 text-[#e3ab1c]',
};

const PRIORITY_TEXT_CLASSES: Record<TicketPriority, string> = {
  [TicketPriority.Low]: 'text-[#a3a3a3]',
  [TicketPriority.Medium]: 'text-[#e9bb49]',
  [TicketPriority.High]: 'text-[#e3ab1c]',
};

export const getPriorityLabel = (p: TicketPriority): string => PRIORITY_LABELS[p] ?? '—';
export const getPriorityBadgeClass = (p: TicketPriority): string => PRIORITY_BADGE_CLASSES[p] ?? '';
export const getPriorityTextClass = (p: TicketPriority): string => PRIORITY_TEXT_CLASSES[p] ?? '';

// ---------- Department ----------
const DEPARTMENT_LABELS: Record<TicketDepartment, string> = {
  [TicketDepartment.Management]: 'مدیریت',
  [TicketDepartment.Technical]: 'فنی',
  [TicketDepartment.Support]: 'پشتیبانی',
  [TicketDepartment.Sale]: 'فروش',
};

export const getDepartmentLabel = (d: TicketDepartment): string => DEPARTMENT_LABELS[d] ?? '—';

// ---------- Options (برای دکمه‌های فیلتر) ----------
export const STEP_OPTIONS = [
  TicketingStep.ProblemDesc,
  TicketingStep.RelevantSection,
  TicketingStep.ProblemPriority,
  TicketingStep.ProblemTitle,
  TicketingStep.Confirmation,
].map((value) => ({ value, label: STEP_LABELS[value] }));
export const DEPARTMENT_OPTIONS = [
  TicketDepartment.Support,
  TicketDepartment.Sale,
  TicketDepartment.Technical,
  TicketDepartment.Management,
].map((value) => ({ value, label: DEPARTMENT_LABELS[value] }));

export const STATUS_OPTIONS = [
  TicketStatus.Open,
  TicketStatus.InProgress,
  TicketStatus.Answered,
  TicketStatus.Closed,
].map((value) => ({ value, label: STATUS_LABELS[value] }));

// ---------- Date ----------
const faDate = new Intl.DateTimeFormat('fa-IR-u-ca-persian', {
  year: 'numeric',
  month: '2-digit',
  day: '2-digit',
});

export const formatDate = (iso: string | Date | null | undefined): string => {
  if (!iso) return '—';
  const d = iso instanceof Date ? iso : new Date(iso);
  return Number.isNaN(d.getTime()) ? '—' : faDate.format(d);
};
