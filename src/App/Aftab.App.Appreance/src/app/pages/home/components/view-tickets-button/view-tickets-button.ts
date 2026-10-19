import { Component, input, signal } from '@angular/core';
import { Input } from 'postcss';
import { RouterLink } from '@angular/router';

@Component({
  imports: [RouterLink],
  selector: 'app-view-tickets-button',
  styleUrl: './view-tickets-button.css',
  templateUrl: './view-tickets-button.html',
})
export class ViewTicketsButton {
  /** Button text – default is “مشاهده تیکت‌ها” */
  label = input('مشاهده تیکت‌ها');

  /** Router link – default goes to tickets list */
  link = input('/ticket-list');

  readonly isHovered = signal(false);
  readonly isPressed = signal(false);
}
