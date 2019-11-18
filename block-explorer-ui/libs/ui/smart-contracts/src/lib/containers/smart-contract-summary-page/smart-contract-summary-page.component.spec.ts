import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SmartContractSummaryPageComponent } from './smart-contract-summary-page.component';

describe('SmartContractSummaryPageComponent', () => {
  let component: SmartContractSummaryPageComponent;
  let fixture: ComponentFixture<SmartContractSummaryPageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SmartContractSummaryPageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SmartContractSummaryPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
