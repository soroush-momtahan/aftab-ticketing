import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SignOn } from './sign-on';

describe('SignOn', () => {
  let component: SignOn;
  let fixture: ComponentFixture<SignOn>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SignOn],
    }).compileComponents();

    fixture = TestBed.createComponent(SignOn);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
