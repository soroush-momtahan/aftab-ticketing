import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AddPersonToOrgan } from './add-person-to-organ';

describe('AddPersonToOrgan', () => {
  let component: AddPersonToOrgan;
  let fixture: ComponentFixture<AddPersonToOrgan>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddPersonToOrgan],
    }).compileComponents();

    fixture = TestBed.createComponent(AddPersonToOrgan);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
