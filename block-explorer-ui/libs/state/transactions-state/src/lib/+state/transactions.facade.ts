import { Injectable } from '@angular/core';

import { select, Store } from '@ngrx/store';

import { TransactionsPartialState } from './transactions.reducer';
import { transactionsQuery } from './transactions.selectors';
import { LoadTransactions, GetAddress, GetAddressDetails } from './transactions.actions';

@Injectable()
export class TransactionsFacade {
  loadedTransactions$ = this.store.pipe(select(transactionsQuery.getLoadedTransactions));
  loadedAddress$ = this.store.pipe(select(transactionsQuery.getLoadedAddress));
  loadedAddressDetails$ = this.store.pipe(select(transactionsQuery.getLoadedAddressDetails));
  selectedAddress$ = this.store.pipe(select(transactionsQuery.getSelectedAddress));
  selectedAddressDetails$ = this.store.pipe(select(transactionsQuery.getSelectedAddressDetails));
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

  getAddressDetails(hash: string) {
    this.store.dispatch(new GetAddressDetails(hash));
  }
}
