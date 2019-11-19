import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, concatMap, debounceTime, switchMap } from 'rxjs/operators';
import { EMPTY, of, asyncScheduler } from 'rxjs';

import * as MainActions from '../actions/main.actions';
import { FinderService } from '../../finder.service';



@Injectable()
export class MainEffects {

   constructor(private actions$: Actions, private finderService: FinderService) { }

   loadGlobal$ = createEffect(() => ({ debounce = 300, scheduler = asyncScheduler } = {}) =>
      this.actions$.pipe(

         ofType(MainActions.identifyEntity),
         debounceTime(debounce, scheduler),
         switchMap(action => {
            return this.finderService.whatIsIt(action.text).pipe(
               map(entity => {
                  if (!!entity) {
                     return MainActions.identified({ payload: entity });
                  }
                  else {
                     return MainActions.identificationError({ error: "Not found" });
                  }
               }),
               catchError(error =>
                  of(MainActions.identificationError({ error }))
               )
            );
         })
      )
   );
}
