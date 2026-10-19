import { Component } from '@angular/core';
import { SupportCta } from './components/support-cta/support-cta';
import { ViewTicketsButton } from './components/view-tickets-button/view-tickets-button';

@Component({
  imports: [SupportCta, ViewTicketsButton],
  selector: 'app-home',
  styleUrl: './home.css',
  templateUrl: './home.html',
})
export class Home {}
