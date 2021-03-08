import { Injectable } from '@angular/core';

import { select, Store } from '@ngrx/store';

import { TransactionsPartialState } from './transactions.reducer';
import { transactionsQuery } from './transactions.selectors';
import { LoadTransactions, GetAddress, GetAddressDetails, GetTransaction, GetBlock, GetBlockHeader, LoadLastBlocks, LoadSmartContractTransactions, LoadStats } from './transactions.actions';

@Injectable()
export class TransactionsFacade {
  loadedTransactions$ = this.store.pipe(select(transactionsQuery.getLoadedTransactions));
  loadedSmartContractTransactions$ = this.store.pipe(select(transactionsQuery.getLoadedSmartContractTransactions));
  loadedStats$ = this.store.pipe(select(transactionsQuery.getLoadedStats));
  lastBlocks$ = this.store.pipe(select(transactionsQuery.getLastBlocks));
  lastBlocksLoaded$ = this.store.pipe(select(transactionsQuery.getLoadedLastBlocks));
  loadedAddress$ = this.store.pipe(select(transactionsQuery.getLoadedAddress));
  loadedAddressDetails$ = this.store.pipe(select(transactionsQuery.getLoadedAddressDetails));
  loadedBlockData$ = this.store.pipe(select(transactionsQuery.getLoadedBlockData));
  selectedAddress$ = this.store.pipe(select(transactionsQuery.getSelectedAddress));
  selectedTransaction$ = this.store.pipe(select(transactionsQuery.getSelectedTransaction));
  selectedBlock$ = this.store.pipe(select(transactionsQuery.getSelectedBlock));
  selectedBlockHeader$ = this.store.pipe(select(transactionsQuery.getSelectedBlockHeader));
  selectedAddressDetails$ = this.store.pipe(select(transactionsQuery.getSelectedAddressDetails));
  allTransactions$ = this.store.pipe(
    select(transactionsQuery.getAllTransactions)
  );
  selectedTransactions$ = this.store.pipe(
    select(transactionsQuery.getSelectedTransactions)
  );
  stats$ = this.store.pipe(
    select(transactionsQuery.getStats)
  );
  smartContractTransactions$ = this.store.pipe(
    select(transactionsQuery.getSmartContractTransactions)
  );

  constructor(private store: Store<TransactionsPartialState>) {}

  loadAll() {
    this.store.dispatch(new LoadTransactions());
  }

  getLastSmartContracts(records: number) {
    this.store.dispatch(new LoadSmartContractTransactions(records));
  }

  getLastBlocks(records: number) {
    this.store.dispatch(new LoadLastBlocks(records));
  }

  getStats() {
    this.store.dispatch(new LoadStats());
  }

  getAddress(hash: string) {
    this.store.dispatch(new GetAddress(hash));
  }

  getTransaction(hash: string) {
    this.store.dispatch(new GetTransaction(hash));
  }

  getBlock(blockHeight: string) {
    this.store.dispatch(new GetBlock(blockHeight));
  }

  getBlockHeader(blockHeight: string) {
    this.store.dispatch(new GetBlockHeader(blockHeight));
  }

  getAddressDetails(hash: string) {
    this.store.dispatch(new GetAddressDetails(hash));
  }
}
