// import { TicketDepartment } from '../enums/ticket-department.enum';
// import { TicketPriority } from '../enums/ticket-priority.enum';
// import { inject, Injectable } from '@angular/core';
// import { HttpClient, HttpParams } from '@angular/common/http';
// import { Observable } from 'rxjs';
// import { TicketStatus } from '../enums/ticket-status.enum';
// import { TicketsPage } from '../models/tickets-page.model';
//
// export interface AddTicketRequest {
//   title: string;
//   content: string;
//   department: TicketDepartment | null;
//   priority: TicketPriority | null;
// }
//
// export interface GetTicketsRequest {
//   searchString: string,
//   sortByPriority: boolean,
//   filterByDepartment?: TicketDepartment | null,
//   filterByStatus?: TicketStatus | null,
//   filterByPersonId?: string | null,
//   pageIndex: number,
//   pageSize: number
// }
//
// export interface GetTicketResponse {
//   ticket: TicketDto;
//   replies: ReplyTicketDto[];
// }
//
// export interface TicketDto {
//   id: string;
//   personId: string;
//   displayName: string;
//   title: string;
//   content: string;
//   priority: TicketPriority;
//   department: TicketDepartment;
//   status: TicketStatus;
//   createdAt: Date | null;
//   modifiedAt: Date | null;
// }
//
// export interface ReplyTicketDto {
//   id: string;
//   replierId: string;
//   replierDisplayName: string;
//   content: string;
// }
//
// @Injectable({ providedIn: 'root' })
// export class TicketingService {
//   private readonly http = inject(HttpClient);
//   private readonly baseUrl = '/api';
//
//   AddTicket(command: AddTicketRequest): Observable<void> {
//     return this.http.post<void>(`${this.baseUrl}/tickets`, command);
//   }
//   getTicket(ticketId: string): Observable<GetTicketResponse> {
//     return this.http.get<GetTicketResponse>(`${this.baseUrl}/tickets/${ticketId}`);
//   }
//   getTickets(req: GetTicketsRequest): Observable<TicketsPage>{
//     let params = new HttpParams()
//       .set('SortByPriority', req.sortByPriority)
//       .set('PageIndex', req.pageIndex)
//       .set('PageSize', req.pageSize);
//
//     if (req.searchString?.trim()) {
//       params = params.set('SearchString', req.searchString.trim());
//     }
//
//     if (req.filterByDepartment != null) {
//       params = params.set('FilterByDepartment', req.filterByDepartment);
//     }
//
//     if (req.filterByStatus != null) {
//       params = params.set('FilterByStatus', req.filterByStatus);
//     }
//
//     if (req.filterByPersonId != null) {
//       params = params.set('FilterByPersonId', req.filterByPersonId);
//     }
//
//     return this.http.get<TicketsPage>(`${this.baseUrl}/Tickets`, { params });
//   }
//   addReply(ticketId: string, content: string, status:TicketStatus): Observable<void>{
//     return this.http.post<void>(
//       `${this.baseUrl}/tickets/${ticketId}/reply?content=${content}&status=${status}`,
//       {}
//     );
//   }
//   changeStatus(ticketId: string, status: TicketStatus): Observable<void> {
//     return this.http.patch<void>(`${this.baseUrl}/tickets/${ticketId}/status`, status);
//   }
//   updateContent(ticketId: string, content: string): Observable<void>{
//     return this.http.patch<void>(`${this.baseUrl}/tickets/${ticketId}/reply`, content);
//   }
//   updateReplyContent(ticketId: string, replyTicketId: string, content: string): Observable<void>{
//     return this.http.patch<void>(
//       `${this.baseUrl}/tickets/${ticketId}/replies/${replyTicketId}/content`,
//       content,
//     );
//   }
//   softDelete(ticketId: string): Observable<void> {
//     return this.http.delete<void>(`${this.baseUrl}/tickets/${ticketId}`);
//   }
//   softDeleteReply(ticketId: string, replyTicketId: string){
//     return this.http.delete<void>(`${this.baseUrl}/tickets/${ticketId}/replies/${replyTicketId}`);
//   }
// }

// مسیر: ticketing/services/ticketing.service.ts
import { TicketDepartment } from '../enums/ticket-department.enum';
import { TicketPriority } from '../enums/ticket-priority.enum';
import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TicketStatus } from '../enums/ticket-status.enum';
import { TicketsPage } from '../models/tickets-page.model';

export interface UpdateReplyVisibilityRequest {
  ticketId: string;
  replyTicketId: string;
  visibility: boolean;
}

export interface AddTicketRequest {
  title: string;
  content: string;
  department: TicketDepartment | null;
  priority: TicketPriority | null;
}

export interface GetTicketsRequest {
  searchString: string;
  sortByPriority: boolean;
  filterByDepartment?: TicketDepartment | null;
  filterByStatus?: TicketStatus | null;
  filterByPersonId?: string | null;
  pageIndex: number;
  pageSize: number;
}

export interface GetTicketResponse {
  ticket: TicketDto;
  replies: ReplyTicketDto[];
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
  createdAt: Date | null;
  modifiedAt: Date | null;
}

export interface ReplyTicketDto {
  id: string;
  replierId: string;
  replierDisplayName: string;
  content: string;
  isVisible: boolean;
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

  getTickets(req: GetTicketsRequest): Observable<TicketsPage> {
    let params = new HttpParams()
      .set('SortByPriority', req.sortByPriority)
      .set('PageIndex', req.pageIndex)
      .set('PageSize', req.pageSize);

    if (req.searchString?.trim()) params = params.set('SearchString', req.searchString.trim());
    if (req.filterByDepartment != null)
      params = params.set('FilterByDepartment', req.filterByDepartment);
    if (req.filterByStatus != null) params = params.set('FilterByStatus', req.filterByStatus);
    if (req.filterByPersonId != null) params = params.set('FilterByPersonId', req.filterByPersonId);

    return this.http.get<TicketsPage>(`${this.baseUrl}/Tickets`, { params });
  }

  // HttpParams مقدار را encode می‌کند (قبلا با & # % و ... در متن، پارامتر خراب می‌شد).
  // پیشنهاد: content را به body منتقل کنید (بخش توضیح پایین را ببینید).
  addReply(ticketId: string, content: string, status: TicketStatus): Observable<void> {
    const params = new HttpParams().set('content', content).set('status', status);
    return this.http.post<void>(`${this.baseUrl}/tickets/${ticketId}/reply`, null, { params });
  }

  changeStatus(ticketId: string, status: TicketStatus): Observable<void> {
    return this.http.patch<void>(`${this.baseUrl}/tickets/${ticketId}/status`, status);
  }

  updateContent(ticketId: string, content: string): Observable<void> {
    return this.http.patch<void>(`${this.baseUrl}/tickets/${ticketId}/reply`, content);
  }

  updateReplyContent(
    ticketId: string,
    replyTicketId: string,
    newContent: string,
  ): Observable<void> {
    let params = new HttpParams().set('newContent', newContent);
    return this.http.patch<void>(
      `${this.baseUrl}/tickets/${ticketId}/replies/${replyTicketId}/content`,
      null,
      { params },
    );
  }
  updateReplyVisibility(req: UpdateReplyVisibilityRequest) {
    let params = new HttpParams().set('visibility', req.visibility);
    return this.http.patch<void>(
      `${this.baseUrl}/tickets/${req.ticketId}/replies/${req.replyTicketId}/visibility`,
      null,
      { params },
    );
  }

  softDelete(ticketId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/tickets/${ticketId}`);
  }

  softDeleteReply(ticketId: string, replyTicketId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/tickets/${ticketId}/replies/${replyTicketId}`);
  }
}
