import { async, TestBed } from '@angular/core/testing';
import { StateTransactionsStateModule } from './state-transactions-state.module';

describe('StateTransactionsStateModule', () => {
  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [StateTransactionsStateModule]
    }).compileComponents();
  }));

  it('should create', () => {
    expect(StateTransactionsStateModule).toBeDefined();
  });
});
