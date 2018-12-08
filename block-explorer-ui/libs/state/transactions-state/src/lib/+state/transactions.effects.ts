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
import { TransactionsService } from '../services/transactions.service';
import { map } from 'rxjs/operators';

@Injectable()
export class TransactionsEffects {
  @Effect() loadTransactions$ = this.dataPersistence.fetch(
    TransactionsActionTypes.LoadTransactions,
    {
      run: (action: LoadTransactions, state: TransactionsPartialState) => {
        this.transactionsService.transactions().pipe(
          map((transactions) => {
            return new TransactionsLoaded(transactions);
          })
        );
      },

      onError: (action: LoadTransactions, error) => {
        console.error('Error', error);
        return new TransactionsLoadError(error);
      }
    }
  );

  constructor(
    private actions$: Actions,
    private dataPersistence: DataPersistence<TransactionsPartialState>,
    private transactionsService: TransactionsService
  ) {}
}
