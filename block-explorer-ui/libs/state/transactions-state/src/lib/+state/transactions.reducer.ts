import {
  TransactionsAction,
  TransactionsActionTypes
} from './transactions.actions';

export const TRANSACTIONS_FEATURE_KEY = 'transactions';

/**
 * Interface for the 'Transactions' data used in
 *  - TransactionsState, and
 *  - transactionsReducer
 *
 *  Note: replace if already defined in another module
 */

/* tslint:disable:no-empty-interface */
export interface Entity {}

export interface TransactionsState {
  list: Entity[]; // list of Transactions; analogous to a sql normalized table
  selectedId?: string | number; // which Transactions record has been selected
  loaded: boolean; // has the Transactions list been loaded
  error?: any; // last none error (if any)
}

export interface TransactionsPartialState {
  readonly [TRANSACTIONS_FEATURE_KEY]: TransactionsState;
}

export const initialState: TransactionsState = {
  list: [],
  loaded: false
};

export function transactionsReducer(
  state: TransactionsState = initialState,
  action: TransactionsAction
): TransactionsState {
  switch (action.type) {
    case TransactionsActionTypes.TransactionsLoaded: {
      state = {
        ...state,
        list: action.payload,
        loaded: true
      };
      break;
    }
  }
  return state;
}
