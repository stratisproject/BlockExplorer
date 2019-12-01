import * as fromSelectedTransaction from './selected-transaction.reducer';
import { ActionReducerMap } from '@ngrx/store';

export const transactionFeatureKey = 'transaction';

export interface TransactionState {
    selectedTransaction: fromSelectedTransaction.SelectedTransactionState
}

export const reducers: ActionReducerMap<TransactionState> = {
    selectedTransaction: fromSelectedTransaction.reducer
};
