import { Component, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Ticket } from '../../../ticket/ticket';

@Component({
  imports: [RouterLink],
  selector: 'app-support-cta',
  styleUrl: './support-cta.css',
  templateUrl: './support-cta.html',
})
export class SupportCta {
  readonly isHovered = signal(false);
  readonly isPressed = signal(false);
  protected readonly Ticket = Ticket;
}
