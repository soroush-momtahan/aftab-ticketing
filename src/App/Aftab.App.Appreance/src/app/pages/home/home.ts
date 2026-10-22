import { Component } from '@angular/core';
import { SupportCta } from './components/support-cta/support-cta';
import { ViewTicketsButton } from './components/view-tickets-button/view-tickets-button';
import { RouterLink } from '@angular/router';

@Component({
  imports: [SupportCta, ViewTicketsButton, RouterLink],
  selector: 'app-home',
  styleUrl: './home.css',
  templateUrl: './home.html',
})
export class Home {}
