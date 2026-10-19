import { Component, inject, signal } from '@angular/core';
import { TicketListStore } from './store/ticket-list.store';
import { TicketingStore } from '../../store/ticketing.store';
import { DEPARTMENT_OPTIONS, STATUS_OPTIONS } from '../../mappers/ticket.mappers';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

@Component({
  imports: [FormsModule, RouterLink],
  providers: [TicketListStore, TicketingStore],
  selector: 'app-ticket-list',
  styleUrl: './ticket-list.css',
  templateUrl: './ticket-list.html',
})
export class TicketList {
  protected readonly store = inject(TicketListStore);

  protected readonly departments = DEPARTMENT_OPTIONS;
  protected readonly statuses = STATUS_OPTIONS;
  protected readonly skeletons = [1, 2, 3, 4, 5];

  protected readonly searchText = signal('');

  protected onSearch(value: string) {
    this.searchText.set(value);
    this.store.search(value);
  }
}
