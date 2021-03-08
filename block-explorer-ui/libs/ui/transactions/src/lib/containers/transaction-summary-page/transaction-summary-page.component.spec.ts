import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TransactionSummaryPageComponent } from './transaction-summary-page.component';

describe('TransactionSummaryPageComponent', () => {
  let component: TransactionSummaryPageComponent;
  let fixture: ComponentFixture<TransactionSummaryPageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TransactionSummaryPageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TransactionSummaryPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
