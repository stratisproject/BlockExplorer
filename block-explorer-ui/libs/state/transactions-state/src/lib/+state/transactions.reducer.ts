import {
  TransactionsAction,
  TransactionsActionTypes
} from './transactions.actions';
import { BalanceSummaryModel, BalanceResponseModel, TransactionModel, TransactionSummaryModel, BlockResponseModel, BlockHeaderResponseModel } from '@blockexplorer/shared/models';

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
  list: TransactionModel[];
  selectedId?: string | number;
  selectedAddress?: BalanceSummaryModel;
  selectedTransaction?: TransactionSummaryModel;
  selectedBlock?: BlockResponseModel;
  selectedBlockHeader?: BlockHeaderResponseModel;
  selectedAddressDetails?: BalanceResponseModel;
  loadedTransactions: boolean;
  loadedAddress: boolean;
  loadedBlockData: boolean;
  loadedAddressDetails: boolean;
  error?: any;
}

export interface TransactionsPartialState {
  readonly [TRANSACTIONS_FEATURE_KEY]: TransactionsState;
}

export const initialState: TransactionsState = {
  list: [],
  selectedAddress: null,
  selectedTransaction: null,
  selectedBlock: null,
  selectedBlockHeader: null,
  selectedAddressDetails: null,
  loadedTransactions: false,
  loadedAddress: false,
  loadedAddressDetails: false,
  loadedBlockData: false
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
    case TransactionsActionTypes.GetTransaction: {
      state = {
        ...state,
        loadedTransactions: false,
      };
      break;
    }
    case TransactionsActionTypes.GetBlockHeader:
    case TransactionsActionTypes.GetBlock: {
      state = {
        ...state,
        selectedBlock: undefined,
        selectedBlockHeader: undefined,
        loadedBlockData: false,
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
    case TransactionsActionTypes.BlockLoaded: {
      state = {
        ...state,
        selectedBlock: action.block,
        loadedBlockData: true
      };
      break;
    }
    case TransactionsActionTypes.BlockHeaderLoaded: {
      state = {
        ...state,
        selectedBlockHeader: action.block,
        loadedBlockData: true
      };
      break;
    }
    case TransactionsActionTypes.TransactionLoaded: {
      state = {
        ...state,
        selectedTransaction: action.transaction,
        loadedTransactions: true
      };
      break;
    }
    case TransactionsActionTypes.TransactionLoadError: {
      state = {
        ...state,
        loadedTransactions: true
      };
      break;
    }
    case TransactionsActionTypes.TransactionLoadError:
    case TransactionsActionTypes.TransactionLoadError: {
      state = {
        ...state,
        loadedBlockData: true
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
