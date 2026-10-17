import { Component } from '@angular/core';
import { SupportCta } from './components/support-cta/support-cta';

@Component({
  imports: [SupportCta],
  selector: 'app-home',
  styleUrl: './home.css',
  templateUrl: './home.html',
})
export class Home {}
