import { Injectable } from '@angular/core';

import { select, Store } from '@ngrx/store';

import { TransactionsPartialState } from './transactions.reducer';
import { transactionsQuery } from './transactions.selectors';
import { LoadTransactions, GetAddress, GetAddressDetails, GetTransaction } from './transactions.actions';

@Injectable()
export class TransactionsFacade {
  loadedTransactions$ = this.store.pipe(select(transactionsQuery.getLoadedTransactions));
  loadedAddress$ = this.store.pipe(select(transactionsQuery.getLoadedAddress));
  loadedAddressDetails$ = this.store.pipe(select(transactionsQuery.getLoadedAddressDetails));
  selectedAddress$ = this.store.pipe(select(transactionsQuery.getSelectedAddress));
  selectedTransaction$ = this.store.pipe(select(transactionsQuery.getSelectedTransaction));
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

  getTransaction(hash: string) {
    this.store.dispatch(new GetTransaction(hash));
  }

  getAddressDetails(hash: string) {
    this.store.dispatch(new GetAddressDetails(hash));
  }
}
