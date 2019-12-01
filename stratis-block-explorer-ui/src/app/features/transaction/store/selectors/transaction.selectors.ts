import { createFeatureSelector, createSelector } from '@ngrx/store';
import * as fromTransaction from '../reducers';

export const selectTransactionState = createFeatureSelector<fromTransaction.TransactionState>(
    fromTransaction.transactionFeatureKey
);

export const getSelectedTransaction = createSelector(selectTransactionState, (state: fromTransaction.TransactionState) =>
    state.selectedTransaction.transaction
);

export const getIsSelectedTransactionLoaded = createSelector(selectTransactionState, (state: fromTransaction.TransactionState) =>
    state.selectedTransaction.isSelected
);

export const getSelectedTransactionError$ = createSelector(selectTransactionState, (state: fromTransaction.TransactionState) =>
    state.selectedTransaction.error
);
