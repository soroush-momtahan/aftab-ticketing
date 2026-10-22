import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TicketReporter } from './ticket-reporter';

describe('TicketReporter', () => {
  let component: TicketReporter;
  let fixture: ComponentFixture<TicketReporter>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TicketReporter],
    }).compileComponents();

    fixture = TestBed.createComponent(TicketReporter);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
