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
const getLoadedAddress = createSelector(getTransactionsState, (state: TransactionsState) => state.loadedAddress);
const getLoadedAddressDetails = createSelector(getTransactionsState, (state: TransactionsState) => state.loadedAddressDetails);
const getSelectedAddress = createSelector(getTransactionsState, (state: TransactionsState) => state.selectedAddress);
const getSelectedTransaction = createSelector(getTransactionsState, (state: TransactionsState) => state.selectedTransaction);
const getSelectedAddressDetails = createSelector(getTransactionsState, (state: TransactionsState) => state.selectedAddressDetails);
const getError = createSelector(getTransactionsState, (state: TransactionsState) => state.error);

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
  getLoadedAddress,
  getLoadedAddressDetails,
  getError,
  getAllTransactions,
  getSelectedAddress,
  getSelectedTransaction,
  getSelectedAddressDetails,
  getSelectedTransactions
};
