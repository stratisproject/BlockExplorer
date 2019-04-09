import { async, TestBed } from '@angular/core/testing';
import { StateSmartContractsStateModule } from './state-smart-contracts-state.module';

describe('StateSmartContractsStateModule', () => {
  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [StateSmartContractsStateModule]
    }).compileComponents();
  }));

  it('should create', () => {
    expect(StateSmartContractsStateModule).toBeDefined();
  });
});
