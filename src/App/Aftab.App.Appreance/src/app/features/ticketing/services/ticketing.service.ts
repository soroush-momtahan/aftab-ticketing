import { TicketDepartment } from '../enums/ticket-department.enum';
import { TicketPriority } from '../enums/ticket-priority.enum';
import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TicketStatus } from '../enums/ticket-status.enum';

export interface AddTicketRequest {
  title: string;
  content: string;
  department: TicketDepartment | null;
  priority: TicketPriority | null;
}

export interface GetTicketsRequest {
  searchString: string,
  sortByPriority: boolean,
  filterByDepartment?: TicketDepartment | null,
  filterByStatus?: TicketStatus | null,
  filterByPersonId?: string | null,
  pageIndex: number,
  pageSize: number
}

export interface GetTicketResponse {
  ticket: TicketDto;
  Replies: ReplyTicketDto[];
}

export interface TicketDto {
  id: string;
  personId: string;
  displayName: string;
  title: string;
  content: string;
  priority: TicketPriority;
  department: TicketDepartment;
  status: TicketStatus;
}

export interface ReplyTicketDto {
  id: string;
  replierId: string;
  replierDisplayName: string;
  content: string;
}

@Injectable({ providedIn: 'root' })
export class TicketingService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api';

  AddTicket(command: AddTicketRequest): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/tickets`, command);
  }
  getTicket(ticketId: string): Observable<GetTicketResponse> {
    return this.http.get<GetTicketResponse>(`${this.baseUrl}/tickets/${ticketId}`);
  }
  getTickets(req: GetTicketsRequest): Observable<GetTicketResponse[]>{
    return this.http.get<GetTicketResponse[]>(
      `${this.baseUrl}/tickets?SearchString=${req.searchString}&SortByPriority=${req.sortByPriority?? false}&FilterByDepartment=${req.filterByDepartment}&FilterByStatus=${req.filterByStatus}&FilterByPersonId=${req.filterByPersonId}&PageIndex=${req.pageIndex}&PageSize=${req.pageSize}`,
    );
  }
  addReply(ticketId: string, content: string): Observable<void>{
    return this.http.post<void>(`${this.baseUrl}/tickets/${ticketId}/reply`, content);
  }
  changeStatus(ticketId: string, status: TicketStatus): Observable<void> {
    return this.http.patch<void>(`${this.baseUrl}/tickets/${ticketId}/status`, status);
  }
  updateContent(ticketId: string, content: string): Observable<void>{
    return this.http.patch<void>(`${this.baseUrl}/tickets/${ticketId}/reply`, content);
  }
  updateReplyContent(ticketId: string, replyTicketId: string, content: string): Observable<void>{
    return this.http.patch<void>(
      `${this.baseUrl}/tickets/${ticketId}/replies/${replyTicketId}/content`,
      content,
    );
  }
  softDelete(ticketId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/tickets/${ticketId}`);
  }
  softDeleteReply(ticketId: string, replyTicketId: string){
    return this.http.delete<void>(`${this.baseUrl}/tickets/${ticketId}/replies/${replyTicketId}`);
  }
}
