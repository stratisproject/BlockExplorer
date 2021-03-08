import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TokenTransactionsTableComponent } from './token-transactions-table.component';

describe('TokenTransactionsTableComponent', () => {
  let component: TokenTransactionsTableComponent;
  let fixture: ComponentFixture<TokenTransactionsTableComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TokenTransactionsTableComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TokenTransactionsTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
