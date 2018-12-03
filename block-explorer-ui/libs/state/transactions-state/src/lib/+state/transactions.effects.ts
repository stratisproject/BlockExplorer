import { Injectable } from '@angular/core';
import { Effect, Actions } from '@ngrx/effects';
import { DataPersistence } from '@nrwl/nx';

import { TransactionsPartialState } from './transactions.reducer';
import {
  LoadTransactions,
  TransactionsLoaded,
  TransactionsLoadError,
  TransactionsActionTypes
} from './transactions.actions';

@Injectable()
export class TransactionsEffects {
  @Effect() loadTransactions$ = this.dataPersistence.fetch(
    TransactionsActionTypes.LoadTransactions,
    {
      run: (action: LoadTransactions, state: TransactionsPartialState) => {
        // Your custom REST 'load' logic goes here. For now just return an empty list...
        return new TransactionsLoaded([]);
      },

      onError: (action: LoadTransactions, error) => {
        console.error('Error', error);
        return new TransactionsLoadError(error);
      }
    }
  );

  constructor(
    private actions$: Actions,
    private dataPersistence: DataPersistence<TransactionsPartialState>
  ) {}
}
