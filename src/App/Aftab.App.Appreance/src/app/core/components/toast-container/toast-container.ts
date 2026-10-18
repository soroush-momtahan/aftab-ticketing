import { Component, inject, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToastService, Toast } from './services/toast.service';

@Component({
  imports: [CommonModule],
  selector: 'app-toast-container',
  styleUrl: './toast-container.css',
  templateUrl: './toast-container.html',
})
export class ToastContainer implements OnDestroy {
  readonly toastService = inject(ToastService);

  iconBgClass(type: Toast['type']): string {
    const map: Record<Toast['type'], string> = {
      success: 'from-porcelain-500/15 to-porcelain-600/5 border-porcelain-500/20',
      error: 'from-red-500/15 to-red-600/5 border-red-500/20',
      warning: 'from-amber-500/15 to-amber-600/5 border-amber-500/20',
      info: 'from-sky-500/15 to-sky-600/5 border-sky-500/20',
      loading: 'from-porcelain-500/10 to-transparent border-porcelain-500/15',
    };
    return map[type];
  }

  onEnter(toast: Toast) {
    if (toast.duration <= 0) return;
    this.toastService.pause(toast.id);
  }

  onLeave(toast: Toast) {
    if (toast.duration <= 0) return;
    this.toastService.resume(toast.id);
  }

  ngOnDestroy() {
    // cleanup if needed
  }
}
