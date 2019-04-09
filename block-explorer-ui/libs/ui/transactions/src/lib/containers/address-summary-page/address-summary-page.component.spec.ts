import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AddressSummaryPageComponent } from './address-summary-page.component';

describe('AddressSummaryPageComponent', () => {
  let component: AddressSummaryPageComponent;
  let fixture: ComponentFixture<AddressSummaryPageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AddressSummaryPageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddressSummaryPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
