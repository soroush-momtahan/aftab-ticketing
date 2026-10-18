// toast-container.service.ts
import { Injectable, signal, computed, inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

export type ToastType = 'success' | 'error' | 'warning' | 'info' | 'loading';

export interface ToastConfig {
  id?: string;
  title?: string;
  message: string;
  type?: ToastType;
  duration?: number; // ms, 0 = sticky
  dismissible?: boolean;
  action?: {
    label: string;
    onClick: () => void;
  };
  icon?: string; // optional custom SVG path or emoji
}

export interface Toast extends Required<Omit<ToastConfig, 'id' | 'action' | 'icon'>> {
  id: string;
  createdAt: number;
  action?: ToastConfig['action'];
  icon?: string;
  paused: boolean;
}

@Injectable({ providedIn: 'root' })
export class ToastService {
  private readonly platformId = inject(PLATFORM_ID);
  private readonly isBrowser = isPlatformBrowser(this.platformId);

  private readonly _toasts = signal<Toast[]>([]);
  readonly toasts = this._toasts.asReadonly();

  readonly hasToasts = computed(() => this._toasts().length > 0);

  show(config: ToastConfig): string {
    const id = config.id ?? crypto.randomUUID();
    const toast: Toast = {
      id,
      title: config.title ?? '',
      message: config.message,
      type: config.type ?? 'info',
      duration: config.duration ?? 4500,
      dismissible: config.dismissible ?? true,
      action: config.action,
      icon: config.icon,
      createdAt: Date.now(),
      paused: false,
    };

    this._toasts.update((list) => [...list, toast]);

    if (toast.duration > 0 && this.isBrowser) {
      this.scheduleDismiss(id, toast.duration);
    }

    return id;
  }

  success(message: string, title = 'Success', duration = 4000) {
    return this.show({ message, title, type: 'success', duration });
  }

  error(message: string, title = 'Error', duration = 6000) {
    return this.show({ message, title, type: 'error', duration });
  }

  warning(message: string, title = 'Warning', duration = 5000) {
    return this.show({ message, title, type: 'warning', duration });
  }

  info(message: string, title = 'Info', duration = 4500) {
    return this.show({ message, title, type: 'info', duration });
  }

  loading(message: string, title = 'Processing') {
    return this.show({ message, title, type: 'loading', duration: 0, dismissible: false });
  }

  dismiss(id: string) {
    this._toasts.update((list) => list.filter((t) => t.id !== id));
  }

  dismissAll() {
    this._toasts.set([]);
  }

  pause(id: string) {
    this._toasts.update((list) => list.map((t) => (t.id === id ? { ...t, paused: true } : t)));
  }

  resume(id: string) {
    this._toasts.update((list) => list.map((t) => (t.id === id ? { ...t, paused: false } : t)));
  }

  private timers = new Map<string, ReturnType<typeof setTimeout>>();

  private scheduleDismiss(id: string, duration: number) {
    if (!this.isBrowser) return;

    const timer = setTimeout(() => {
      this.dismiss(id);
      this.timers.delete(id);
    }, duration);

    this.timers.set(id, timer);
  }

  // Called by the component when user hovers
  resetTimer(id: string, remaining: number) {
    if (!this.isBrowser) return;
    const existing = this.timers.get(id);
    if (existing) clearTimeout(existing);
    this.scheduleDismiss(id, remaining);
  }
}
