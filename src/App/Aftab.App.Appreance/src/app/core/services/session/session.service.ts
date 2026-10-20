import {computed, inject, Injectable, signal} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {TokenService} from '../token/token.service';
import {catchError, Observable, of, tap} from 'rxjs';

export interface Session {
  id: string;
  userName: string | null;
  roles: string[];
  permissions: string[];
}

@Injectable({ providedIn: 'root' })
export class SessionService {
  private readonly http = inject(HttpClient);
  private readonly tokens = inject(TokenService);

  private readonly _session = signal<Session | null>(null);
  readonly session = this._session.asReadonly();

  readonly isAuthenticated = computed(() => this._session() !== null);
  readonly roles = computed(() => this._session()?.roles ?? []);
  readonly isAdmin = computed(() => this.hasAnyRole('Agent'));

  private readonly permissionSet = computed(() => new Set(this._session()?.permissions ?? []));

  hasPermission(p: string): boolean { return this.permissionSet().has(p); }
  hasRole(r: string): boolean { return this.roles().includes(r); }
  hasAnyRole(...rs: string[]): boolean { return rs.some(r => this.hasRole(r)); }

  /** بعد از دریافت توکن (لاگین/ثبت‌نام) صدا زده می‌شود */
  signIn(token: string): Observable<Session | null> {
    this.tokens.setToken(token);
    return this.refresh();
  }

  refresh(): Observable<Session | null> {
    if (!this.tokens.getToken()) {
      this._session.set(null);
      return of(null);
    }
    return this.http.get<Session>('/api/identity/me').pipe(
      catchError(() => of(null)),
      tap(s => this._session.set(s)),
    );
  }

  logout(): void {
    this.tokens.removeToken();
    this._session.set(null);
  }
}
