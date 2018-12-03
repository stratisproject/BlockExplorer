import { createFeatureSelector, createSelector } from '@ngrx/store';
import {
  TRANSACTIONS_FEATURE_KEY,
  TransactionsState
} from './transactions.reducer';

// Lookup the 'Transactions' feature state managed by NgRx
const getTransactionsState = createFeatureSelector<TransactionsState>(
  TRANSACTIONS_FEATURE_KEY
);

const getLoaded = createSelector(
  getTransactionsState,
  (state: TransactionsState) => state.loaded
);
const getError = createSelector(
  getTransactionsState,
  (state: TransactionsState) => state.error
);

const getAllTransactions = createSelector(
  getTransactionsState,
  getLoaded,
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
  getLoaded,
  getError,
  getAllTransactions,
  getSelectedTransactions
};
