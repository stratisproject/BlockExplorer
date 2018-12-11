import { Injectable } from '@angular/core';
import { Effect, Actions } from '@ngrx/effects';
import { DataPersistence } from '@nrwl/nx';

import { GlobalPartialState } from './global.reducer';
import {
  IndentifyEntity,
  Identified,
  IdentificationError,
  GlobalActionTypes
} from './global.actions';
import { FinderService } from '../services/finder.service';
import { map } from 'rxjs/operators';

@Injectable()
export class GlobalEffects {
  @Effect() loadGlobal$ = this.dataPersistence.fetch(
    GlobalActionTypes.IndentifyEntity,
    {
      run: (action: IndentifyEntity, state: GlobalPartialState) => {
        return this.finderService.whatIsIt(action.id).pipe(map(entity => {
          if (!!entity) {
            return new Identified(entity);
          }

          return new IdentificationError('Not found');
        }));

      },

      onError: (action: IndentifyEntity, error) => {
        console.error('Error', error);
        return new IdentificationError(error);
      }
    }
  );

  constructor(
    private actions$: Actions,
    private dataPersistence: DataPersistence<GlobalPartialState>,
    private finderService: FinderService
  ) {}
}
