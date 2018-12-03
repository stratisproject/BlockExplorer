import { async, TestBed } from '@angular/core/testing';
import { UiSmartContractsModule } from './ui-smart-contracts.module';

describe('UiSmartContractsModule', () => {
  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [UiSmartContractsModule]
    }).compileComponents();
  }));

  it('should create', () => {
    expect(UiSmartContractsModule).toBeDefined();
  });
});
