import { Action } from '@ngrx/store';
import { Entity } from './transactions.reducer';
import { TransactionModel } from 'gen/nswag';
import { BalanceSummaryModel } from '@blockexplorer/shared/models';

export enum TransactionsActionTypes {
  LoadTransactions = '[Transactions] Load Transactions',
  GetAddress = '[Transactions] Get Address',
  TransactionsLoaded = '[Transactions] Transactions Loaded',
  AddressLoaded = '[Transactions] Address Loaded',
  TransactionsLoadError = '[Transactions] Transactions Load Error',
  AddressLoadError = '[Transactions] Address Load Error'
}

export class LoadTransactions implements Action {
  readonly type = TransactionsActionTypes.LoadTransactions;
}

export class GetAddress implements Action {
  readonly type = TransactionsActionTypes.GetAddress;
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

export class AddressLoaded implements Action {
  readonly type = TransactionsActionTypes.AddressLoaded;
  constructor(public address: BalanceSummaryModel) {}
}

export class TransactionsLoaded implements Action {
  readonly type = TransactionsActionTypes.TransactionsLoaded;
  constructor(public transactions: TransactionModel[]) {}
}

export type TransactionsAction =
  | LoadTransactions
  | GetAddress
  | TransactionsLoaded
  | TransactionsLoadError
  | AddressLoaded
  | AddressLoadError;

export const fromTransactionsActions = {
  LoadTransactions,
  GetAddress,
  TransactionsLoaded,
  TransactionsLoadError,
  AddressLoaded,
  AddressLoadError
};
