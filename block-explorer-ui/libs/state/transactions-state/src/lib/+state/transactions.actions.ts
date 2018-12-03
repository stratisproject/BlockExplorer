import { Action } from '@ngrx/store';
import { Entity } from './transactions.reducer';

export enum TransactionsActionTypes {
  LoadTransactions = '[Transactions] Load Transactions',
  TransactionsLoaded = '[Transactions] Transactions Loaded',
  TransactionsLoadError = '[Transactions] Transactions Load Error'
}

export class LoadTransactions implements Action {
  readonly type = TransactionsActionTypes.LoadTransactions;
}

export class TransactionsLoadError implements Action {
  readonly type = TransactionsActionTypes.TransactionsLoadError;
  constructor(public payload: any) {}
}

export class TransactionsLoaded implements Action {
  readonly type = TransactionsActionTypes.TransactionsLoaded;
  constructor(public payload: Entity[]) {}
}

export type TransactionsAction =
  | LoadTransactions
  | TransactionsLoaded
  | TransactionsLoadError;

export const fromTransactionsActions = {
  LoadTransactions,
  TransactionsLoaded,
  TransactionsLoadError
};
