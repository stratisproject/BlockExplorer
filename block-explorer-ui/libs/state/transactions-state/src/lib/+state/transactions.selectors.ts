import { createFeatureSelector, createSelector } from '@ngrx/store';
import {
  TRANSACTIONS_FEATURE_KEY,
  TransactionsState
} from './transactions.reducer';

// Lookup the 'Transactions' feature state managed by NgRx
const getTransactionsState = createFeatureSelector<TransactionsState>(
  TRANSACTIONS_FEATURE_KEY
);

const getLoadedTransactions = createSelector(getTransactionsState, (state: TransactionsState) => state.loadedTransactions);
const getLoadedStats = createSelector(getTransactionsState, (state: TransactionsState) => state.statsLoaded);
const getLoadedSmartContractTransactions = createSelector(getTransactionsState, (state: TransactionsState) => state.smartContractTransactionsLoaded);
const getLastBlocks = createSelector(getTransactionsState, (state: TransactionsState) => state.lastBlocks);
const getLoadedLastBlocks = createSelector(getTransactionsState, (state: TransactionsState) => state.lastBlocksLoaded);
const getLoadedAddress = createSelector(getTransactionsState, (state: TransactionsState) => state.loadedAddress);
const getLoadedAddressDetails = createSelector(getTransactionsState, (state: TransactionsState) => state.loadedAddressDetails);
const getLoadedBlockData = createSelector(getTransactionsState, (state: TransactionsState) => state.loadedBlockData);
const getSelectedAddress = createSelector(getTransactionsState, (state: TransactionsState) => state.selectedAddress);
const getSelectedTransaction = createSelector(getTransactionsState, (state: TransactionsState) => state.selectedTransaction);
const getSelectedAddressDetails = createSelector(getTransactionsState, (state: TransactionsState) => state.selectedAddressDetails);
const getError = createSelector(getTransactionsState, (state: TransactionsState) => state.error);
const getSelectedBlock = createSelector(getTransactionsState, (state: TransactionsState) => state.selectedBlock);
const getSelectedBlockHeader = createSelector(getTransactionsState, (state: TransactionsState) => state.selectedBlockHeader);
const getSmartContractTransactions = createSelector(getTransactionsState, (state: TransactionsState) => state.smartContractTransactions);
const getStats = createSelector(getTransactionsState, (state: TransactionsState) => state.stats);

const getAllTransactions = createSelector(
  getTransactionsState,
  getLoadedTransactions,
  (state: TransactionsState, isLoaded) => {
    return isLoaded ? state.list : [];
  }
);

const getSelectedId = createSelector(
  getTransactionsState,
  (state: TransactionsState) => state.selectedId
);

const getSelectedTransactions = createSelector(
  getAllTransactions,
  getSelectedId,
  (transactions, id) => {
    const result = transactions.find(it => it['id'] === id);
    return result ? Object.assign({}, result) : undefined;
  }
);

export const transactionsQuery = {
  getLoadedTransactions,
  getLoadedStats,
  getLoadedSmartContractTransactions,
  getLastBlocks,
  getLoadedLastBlocks,
  getLoadedAddress,
  getLoadedBlockData,
  getLoadedAddressDetails,
  getError,
  getAllTransactions,
  getSelectedAddress,
  getSelectedBlock,
  getSelectedBlockHeader,
  getSelectedTransaction,
  getSelectedAddressDetails,
  getSelectedTransactions,
  getSmartContractTransactions,
  getStats
};
