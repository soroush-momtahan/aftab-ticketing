import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ViewTicketsButton } from './view-tickets-button';

describe('ViewTicketsButton', () => {
  let component: ViewTicketsButton;
  let fixture: ComponentFixture<ViewTicketsButton>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ViewTicketsButton],
    }).compileComponents();

    fixture = TestBed.createComponent(ViewTicketsButton);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
