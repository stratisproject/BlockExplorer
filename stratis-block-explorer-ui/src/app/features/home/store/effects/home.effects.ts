import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, concatMap, debounceTime, switchMap } from 'rxjs/operators';
import { EMPTY, of, asyncScheduler } from 'rxjs';

import * as HomeActions from '../actions/home.actions';
import { FinderService } from '../../finder.service';



@Injectable()
export class HomeEffects {

  constructor(private actions$: Actions, private finderService: FinderService) { }

  loadGlobal$ = createEffect(() => ({ debounce = 300, scheduler = asyncScheduler } = {}) =>
    this.actions$.pipe(

      ofType(HomeActions.identifyEntity),
      debounceTime(debounce, scheduler),
      switchMap(action => {
        return this.finderService.whatIsIt(action.text).pipe(
          map(entity => {
            if (!!entity) {
              return HomeActions.identified(entity);
            }
            else {
              return HomeActions.identificationError({ error: "Not found" });
            }
          }),
          catchError(error =>
            of(HomeActions.identificationError({ error }))
          )
        );
      })
    )
  );
}
