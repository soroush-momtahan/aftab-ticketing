import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TestToast } from './test-toast';

describe('TestToast', () => {
  let component: TestToast;
  let fixture: ComponentFixture<TestToast>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TestToast],
    }).compileComponents();

    fixture = TestBed.createComponent(TestToast);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
