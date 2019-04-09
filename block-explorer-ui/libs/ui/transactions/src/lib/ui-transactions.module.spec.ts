import { async, TestBed } from '@angular/core/testing';
import { UiTransactionsModule } from './ui-transactions.module';

describe('UiTransactionsModule', () => {
  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [UiTransactionsModule]
    }).compileComponents();
  }));

  it('should create', () => {
    expect(UiTransactionsModule).toBeDefined();
  });
});
