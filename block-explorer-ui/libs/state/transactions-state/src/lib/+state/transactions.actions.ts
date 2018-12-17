import { Action } from '@ngrx/store';
import { Entity } from './transactions.reducer';
import { TransactionModel, TransactionSummaryModel } from 'gen/nswag';
import { BalanceSummaryModel, BalanceResponseModel } from '@blockexplorer/shared/models';

export enum TransactionsActionTypes {
  LoadTransactions = '[Transactions] Load Transactions',
  GetAddress = '[Transactions] Get Address',
  GetTransaction = '[Transactions] Get Transaction',
  GetAddressDetails = '[Transactions] Get Address Details',
  TransactionsLoaded = '[Transactions] Transactions Loaded',
  AddressLoaded = '[Transactions] Address Loaded',
  TransactionLoaded = '[Transactions] Transaction Loaded',
  AddressDetailsLoaded = '[Transactions] Address Details Loaded',
  TransactionsLoadError = '[Transactions] Transactions Load Error',
  AddressLoadError = '[Transactions] Address Load Error',
  TransactionLoadError = '[Transactions] Transaction Load Error',
  AddressDetailsLoadError = '[Transactions] Address Details Load Error'
}

export class LoadTransactions implements Action {
  readonly type = TransactionsActionTypes.LoadTransactions;
}

export class GetAddress implements Action {
  readonly type = TransactionsActionTypes.GetAddress;
  constructor(public addressHash: string) {}
}

export class GetTransaction implements Action {
  readonly type = TransactionsActionTypes.GetTransaction;
  constructor(public hash: string) {}
}

export class GetAddressDetails implements Action {
  readonly type = TransactionsActionTypes.GetAddressDetails;
  constructor(public addressHash: string) {}
}

export class TransactionsLoadError implements Action {
  readonly type = TransactionsActionTypes.TransactionsLoadError;
  constructor(public payload: any) {}
}

export class AddressLoadError implements Action {
  readonly type = TransactionsActionTypes.AddressLoadError;
  constructor(public payload: any) {}
}

export class TransactionLoadError implements Action {
  readonly type = TransactionsActionTypes.TransactionLoadError;
  constructor(public payload: any) {}
}

export class AddressDetailsLoadError implements Action {
  readonly type = TransactionsActionTypes.AddressDetailsLoadError;
  constructor(public payload: any) {}
}

export class AddressLoaded implements Action {
  readonly type = TransactionsActionTypes.AddressLoaded;
  constructor(public address: BalanceSummaryModel) {}
}

export class TransactionLoaded implements Action {
  readonly type = TransactionsActionTypes.TransactionLoaded;
  constructor(public transaction: TransactionSummaryModel) {}
}

export class AddressDetailsLoaded implements Action {
  readonly type = TransactionsActionTypes.AddressDetailsLoaded;
  constructor(public address: BalanceResponseModel) {}
}

export class TransactionsLoaded implements Action {
  readonly type = TransactionsActionTypes.TransactionsLoaded;
  constructor(public transactions: TransactionModel[]) {}
}

export type TransactionsAction =
  | LoadTransactions
  | GetAddress
  | GetTransaction
  | GetAddressDetails
  | TransactionsLoaded
  | TransactionsLoadError
  | AddressLoaded
  | TransactionLoaded
  | AddressLoadError
  | AddressDetailsLoaded
  | TransactionLoadError
  | AddressDetailsLoadError;

export const fromTransactionsActions = {
  LoadTransactions,
  GetAddress,
  GetTransaction,
  GetAddressDetails,
  TransactionsLoaded,
  TransactionsLoadError,
  AddressLoaded,
  TransactionLoaded,
  AddressLoadError,
  AddressDetailsLoaded,
  TransactionLoadError,
  AddressDetailsLoadError
};
