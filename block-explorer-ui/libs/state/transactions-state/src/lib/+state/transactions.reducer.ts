import {
  TransactionsAction,
  TransactionsActionTypes
} from './transactions.actions';
import { BalanceSummaryModel, BalanceResponseModel, TransactionModel } from '@blockexplorer/shared/models';

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
  list: TransactionModel[]; // list of Transactions; analogous to a sql normalized table
  selectedId?: string | number; // which Transactions record has been selected
  selectedAddress?: BalanceSummaryModel;
  selectedAddressDetails?: BalanceResponseModel;
  loadedTransactions: boolean;
  loadedAddress: boolean;
  loadedAddressDetails: boolean;
  error?: any;
}

export interface TransactionsPartialState {
  readonly [TRANSACTIONS_FEATURE_KEY]: TransactionsState;
}

export const initialState: TransactionsState = {
  list: [],
  selectedAddress: null,
  selectedAddressDetails: null,
  loadedTransactions: false,
  loadedAddress: false,
  loadedAddressDetails: false
};

export function transactionsReducer(
  state: TransactionsState = initialState,
  action: TransactionsAction
): TransactionsState {
  switch (action.type) {
    case TransactionsActionTypes.LoadTransactions: {
      state = {
        ...state,
        loadedTransactions: false,
      };
      break;
    }
    case TransactionsActionTypes.TransactionsLoaded: {
      state = {
        ...state,
        list: action.transactions,
        loadedTransactions: true,
      };
      break;
    }
    case TransactionsActionTypes.GetAddress: {
      state = {
        ...state,
        loadedAddress: false,
      };
      break;
    }
    case TransactionsActionTypes.AddressLoaded: {
      state = {
        ...state,
        selectedAddress: action.address,
        loadedAddress: true
      };
      break;
    }
    case TransactionsActionTypes.GetAddressDetails: {
      state = {
        ...state,
        loadedAddressDetails: false,
      };
      break;
    }
    case TransactionsActionTypes.AddressDetailsLoaded: {
      state = {
        ...state,
        selectedAddressDetails: action.address,
        loadedAddressDetails: true
      };
      break;
    }
  }
  return state;
}
