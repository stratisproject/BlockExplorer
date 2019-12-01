import { Action, createReducer, on } from '@ngrx/store';
import * as TransactionActions from '../actions/transaction.actions';
import { Transaction } from '../../models';

export interface SelectedTransactionState {
    transaction: Transaction,
    isSelected,
    error: Error | string
}

export const initialState: SelectedTransactionState = {
    transaction: null,
    isSelected: false,
    error: null
};

const selectedTransactionReducer = createReducer(
    initialState,

    on(TransactionActions.loadTransaction, state => state = ({
        ...state,
        isSelected: false,
        error: null
    })),

    on(TransactionActions.transactionLoaded, (state, action) => ({
        ...state,
        isSelected: true,
        transaction: action.transaction,
        error: null
    })),

    on(TransactionActions.loadTransactionError, (state, action) => ({
        ...state,
        isSelected: false,
        transaction: null,
        error: action.error
    }))
);

export function reducer(state: SelectedTransactionState | undefined, action: Action) {
    return selectedTransactionReducer(state, action);
}
