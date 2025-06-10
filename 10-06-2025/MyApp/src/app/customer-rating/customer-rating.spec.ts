import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomerRating } from './customer-rating';

describe('CustomerRating', () => {
  let component: CustomerRating;
  let fixture: ComponentFixture<CustomerRating>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CustomerRating]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CustomerRating);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
