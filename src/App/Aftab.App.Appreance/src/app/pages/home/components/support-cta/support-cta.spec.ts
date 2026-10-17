import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SupportCta } from './support-cta';

describe('SupportCta', () => {
  let component: SupportCta;
  let fixture: ComponentFixture<SupportCta>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SupportCta],
    }).compileComponents();

    fixture = TestBed.createComponent(SupportCta);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
