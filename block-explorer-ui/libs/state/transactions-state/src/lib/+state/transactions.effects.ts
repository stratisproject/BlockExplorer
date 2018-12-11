import { Injectable } from '@angular/core';
import { Effect, Actions } from '@ngrx/effects';
import { DataPersistence } from '@nrwl/nx';

import { TransactionsPartialState } from './transactions.reducer';
import {
  LoadTransactions,
  TransactionsLoaded,
  TransactionsLoadError,
  TransactionsActionTypes,
  GetAddress,
  AddressLoaded,
  AddressLoadError
} from './transactions.actions';
import { TransactionsService } from '../services/transactions.service';
import { map } from 'rxjs/operators';
import { BalancesService } from '../services/balances.service';

@Injectable()
export class TransactionsEffects {
  @Effect() loadTransactions$ = this.dataPersistence.fetch(
    TransactionsActionTypes.LoadTransactions,
    {
      run: (action: LoadTransactions, state: TransactionsPartialState) => {
        return this.transactionsService.transactions().pipe(
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

  @Effect() getAddress$ = this.dataPersistence.fetch(
    TransactionsActionTypes.GetAddress,
    {
      run: (action: GetAddress, state: TransactionsPartialState) => {
        return this.balancesService.addressBalanceSummary(action.addressHash, null, false, false).pipe(
          map((balance) => {
            return new AddressLoaded(balance);
          })
        );
      },

      onError: (action: GetAddress, error) => {
        console.error('Error', error);
        return new AddressLoadError(error);
      }
    }
  );

  constructor(
    private actions$: Actions,
    private dataPersistence: DataPersistence<TransactionsPartialState>,
    private transactionsService: TransactionsService,
    private balancesService: BalancesService
  ) {}
}
