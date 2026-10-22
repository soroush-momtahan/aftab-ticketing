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
  label = input('مشاهده تیکت‌ها');
  link = input('/ticket-list');

  readonly isHovered = signal(false);
  readonly isPressed = signal(false);
}
