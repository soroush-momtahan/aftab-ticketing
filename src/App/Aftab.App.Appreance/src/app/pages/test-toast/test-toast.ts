import { Component, inject } from '@angular/core';
import { ToastService } from '../../core/components/toast-container/services/toast.service';

@Component({
  imports: [],
  selector: 'app-test-toast',
  styleUrl: './test-toast.css',
  templateUrl: './test-toast.html',
})
export class TestToast {
  private toast = inject(ToastService);

  demo() {
    this.toast.error(
      'Ceramic compound brake pads have been added to your cart.',
      'Order Confirmed',
    );
    console.log('hello');
    // or more advanced:
    this.toast.show({
      type: 'warning',
      title: 'Stock Alert',
      message: 'Only 3 sets remaining for Porsche 911 GT3 RS pads.',
      duration: 7000,
      action: {
        label: 'Reserve now',
        onClick: () => console.log('Reserved'),
      },
    });
  }
}
