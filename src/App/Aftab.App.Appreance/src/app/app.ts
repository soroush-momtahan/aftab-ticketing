import { Component, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ToastContainer } from './core/components/toast-container/toast-container';
import { UserService } from './core/services/user/user.service';
import { AuthService } from './core/services/auth/auth.service';

@Component({
  imports: [RouterOutlet, ToastContainer],
  selector: 'app-root',
  styleUrl: './app.css',
  templateUrl: './app.html',
})
export class App {
  protected readonly title = signal('Aftab.App.Appreance');
  userService = inject(UserService);
  authService = inject(AuthService);
  onLogOut() {
    this.userService.clearUser();
    this.authService.logout();
  }
}
