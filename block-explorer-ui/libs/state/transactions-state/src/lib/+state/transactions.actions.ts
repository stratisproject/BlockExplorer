import { Action } from '@ngrx/store';
import { Entity } from './transactions.reducer';
import { TransactionModel } from 'gen/nswag';
import { BalanceSummaryModel, BalanceResponseModel } from '@blockexplorer/shared/models';

export enum TransactionsActionTypes {
  LoadTransactions = '[Transactions] Load Transactions',
  GetAddress = '[Transactions] Get Address',
  GetAddressDetails = '[Transactions] Get Address Details',
  TransactionsLoaded = '[Transactions] Transactions Loaded',
  AddressLoaded = '[Transactions] Address Loaded',
  AddressDetailsLoaded = '[Transactions] Address Details Loaded',
  TransactionsLoadError = '[Transactions] Transactions Load Error',
  AddressLoadError = '[Transactions] Address Load Error',
  AddressDetailsLoadError = '[Transactions] Address Details Load Error'
}

export class LoadTransactions implements Action {
  readonly type = TransactionsActionTypes.LoadTransactions;
}

export class GetAddress implements Action {
  readonly type = TransactionsActionTypes.GetAddress;
  constructor(public addressHash: string) {}
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

export class AddressDetailsLoadError implements Action {
  readonly type = TransactionsActionTypes.AddressDetailsLoadError;
  constructor(public payload: any) {}
}

export class AddressLoaded implements Action {
  readonly type = TransactionsActionTypes.AddressLoaded;
  constructor(public address: BalanceSummaryModel) {}
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
  | GetAddressDetails
  | TransactionsLoaded
  | TransactionsLoadError
  | AddressLoaded
  | AddressLoadError
  | AddressDetailsLoaded
  | AddressDetailsLoadError;

export const fromTransactionsActions = {
  LoadTransactions,
  GetAddress,
  GetAddressDetails,
  TransactionsLoaded,
  TransactionsLoadError,
  AddressLoaded,
  AddressLoadError,
  AddressDetailsLoaded,
  AddressDetailsLoadError
};
