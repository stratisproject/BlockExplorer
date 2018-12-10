import { Injectable } from '@angular/core';

import { select, Store } from '@ngrx/store';

import { TransactionsPartialState } from './transactions.reducer';
import { transactionsQuery } from './transactions.selectors';
import { LoadTransactions, GetAddress } from './transactions.actions';

@Injectable()
export class TransactionsFacade {
  loaded$ = this.store.pipe(select(transactionsQuery.getLoaded));
  selectedAddress$ = this.store.pipe(select(transactionsQuery.getSelectedAddress));
  allTransactions$ = this.store.pipe(
    select(transactionsQuery.getAllTransactions)
  );
  selectedTransactions$ = this.store.pipe(
    select(transactionsQuery.getSelectedTransactions)
  );

  constructor(private store: Store<TransactionsPartialState>) {}

  loadAll() {
    this.store.dispatch(new LoadTransactions());
  }

  getAddress(hash: string) {
    this.store.dispatch(new GetAddress(hash));
  }
}
