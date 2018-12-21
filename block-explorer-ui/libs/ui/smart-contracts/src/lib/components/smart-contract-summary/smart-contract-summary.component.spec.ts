import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SmartContractSummaryComponent } from './smart-contract-summary.component';

describe('SmartContractSummaryComponent', () => {
  let component: SmartContractSummaryComponent;
  let fixture: ComponentFixture<SmartContractSummaryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SmartContractSummaryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SmartContractSummaryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
