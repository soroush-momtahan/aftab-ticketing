import { Routes } from '@angular/router';
import { Login } from './pages/login/login';
import { Home } from './pages/home/home';
import { Ticket } from './pages/ticket/ticket';
import { authGuard } from './core/guard/auth.guard';
import { TestToast } from './pages/test-toast/test-toast';
import { TicketDetail } from './features/ticketing/components/ticket-detail/ticket-detail';
import { TicketList } from './features/ticketing/components/ticket-list/ticket-list';

export const routes: Routes = [
  {
    path: '',
    component: Home,
  },
  {
    path: 'login',
    component: Login,
  },
  {
    path: 'ticketing',
    component: Ticket,
    canActivate: [authGuard],
  },
  {
    path: 'ticket-detail/:id',
    component: TicketDetail,
  },
  {
    path: 'ticket-list',
    component: TicketList,
  },
  {
    path: 'test',
    component: TestToast,
  },
];
