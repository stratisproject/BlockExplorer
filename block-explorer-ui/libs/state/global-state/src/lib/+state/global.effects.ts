import { Injectable } from '@angular/core';
import { Effect, Actions } from '@ngrx/effects';
import { DataPersistence } from '@nrwl/nx';

import { GlobalPartialState } from './global.reducer';
import {
  LoadGlobal,
  GlobalLoaded,
  GlobalLoadError,
  GlobalActionTypes
} from './global.actions';

@Injectable()
export class GlobalEffects {
  @Effect() loadGlobal$ = this.dataPersistence.fetch(
    GlobalActionTypes.LoadGlobal,
    {
      run: (action: LoadGlobal, state: GlobalPartialState) => {
        // Your custom REST 'load' logic goes here. For now just return an empty list...
        return new GlobalLoaded([]);
      },

      onError: (action: LoadGlobal, error) => {
        console.error('Error', error);
        return new GlobalLoadError(error);
      }
    }
  );

  constructor(
    private actions$: Actions,
    private dataPersistence: DataPersistence<GlobalPartialState>
  ) {}
}
