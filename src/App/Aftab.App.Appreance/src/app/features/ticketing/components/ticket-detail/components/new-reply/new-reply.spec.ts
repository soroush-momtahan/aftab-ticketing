import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NewReply } from './new-reply';

describe('NewReply', () => {
  let component: NewReply;
  let fixture: ComponentFixture<NewReply>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NewReply],
    }).compileComponents();

    fixture = TestBed.createComponent(NewReply);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
