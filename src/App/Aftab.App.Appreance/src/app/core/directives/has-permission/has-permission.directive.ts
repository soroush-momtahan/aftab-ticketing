import {
  computed,
  Directive,
  effect,
  inject,
  input,
  TemplateRef,
  ViewContainerRef,
} from '@angular/core';
import { SessionService } from '../../services/session/session.service';

@Directive({ selector: '[appHasPermission]' })
export class HasPermissionDirective {
  readonly appHasPermission = input.required<string | string[]>();

  constructor() {
    const tpl = inject(TemplateRef);
    const vcr = inject(ViewContainerRef);
    const session = inject(SessionService);

    const allowed = computed(() => {
      const p = this.appHasPermission();
      return [p].flat().every((x) => session.hasPermission(x));
    });

    effect(() => {
      vcr.clear();
      if (allowed()) vcr.createEmbeddedView(tpl);
    });
  }
}
