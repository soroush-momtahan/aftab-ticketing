import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AddOrganForm } from './add-organ-form';

describe('AddOrganForm', () => {
  let component: AddOrganForm;
  let fixture: ComponentFixture<AddOrganForm>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddOrganForm],
    }).compileComponents();

    fixture = TestBed.createComponent(AddOrganForm);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
