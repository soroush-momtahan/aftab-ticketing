import { TicketDepartment } from '../enums/ticket-department.enum';
import { TicketPriority } from '../enums/ticket-priority.enum';
import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface AddTicketRequest {
  title: string;
  content: string;
  department: TicketDepartment | null;
  priority: TicketPriority | null;
}

@Injectable({ providedIn: 'root' })
export class TicketingService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api';

  AddTicket(command: AddTicketRequest): Observable<void>{
    return this.http.post<void>(`${this.baseUrl}/tickets`, command);
  }
}
