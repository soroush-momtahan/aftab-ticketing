import { Routes } from '@angular/router';
import { Login } from './pages/login/login';
import { Home } from './pages/home/home';
import { Ticket } from './pages/ticket/ticket';

export const routes: Routes = [
  {
    path: '',
    component: Home,
  },
  {
    path: 'sign-on',
    component: Login,
  },
  {
    path: 'ticketing',
    component: Ticket,
  },
];
