import { Component } from '@angular/core';
import { TicketForm } from '../../features/ticketing/components/ticket-form/ticket-form';

@Component({
  imports: [TicketForm],
  selector: 'app-ticket',
  styleUrl: './ticket.css',
  templateUrl: './ticket.html',
})
export class Ticket {}
