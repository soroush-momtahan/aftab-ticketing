import { Component } from '@angular/core';
import { Ticketing } from '../../features/ticketing/ticketing';

@Component({
  imports: [Ticketing],
  selector: 'app-ticket',
  styleUrl: './ticket.css',
  templateUrl: './ticket.html',
})
export class Ticket {}
