import { Injectable } from '@angular/core';
import { Effect, Actions } from '@ngrx/effects';
import { DataPersistence } from '@nrwl/nx';

import { SmartContractsPartialState } from './smart-contracts.reducer';
import {
  LoadSmartContracts,
  SmartContractsLoaded,
  SmartContractsLoadError,
  SmartContractsActionTypes
} from './smart-contracts.actions';

@Injectable()
export class SmartContractsEffects {
  @Effect() loadSmartContracts$ = this.dataPersistence.fetch(
    SmartContractsActionTypes.LoadSmartContracts,
    {
      run: (action: LoadSmartContracts, state: SmartContractsPartialState) => {
        // Your custom REST 'load' logic goes here. For now just return an empty list...
        return new SmartContractsLoaded([]);
      },

      onError: (action: LoadSmartContracts, error) => {
        console.error('Error', error);
        return new SmartContractsLoadError(error);
      }
    }
  );

  constructor(
    private actions$: Actions,
    private dataPersistence: DataPersistence<SmartContractsPartialState>
  ) {}
}
