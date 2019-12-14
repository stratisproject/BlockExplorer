import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, switchMap } from 'rxjs/operators';
import { of } from 'rxjs';

import * as BlocksActions from '../actions/blocks.actions';
import { BlocksService } from '../../services/blocks.service';


@Injectable()
export class BlocksEffects {

   constructor(private actions$: Actions, private blocksService: BlocksService) { }

   loadBlock$ = createEffect(() =>
      this.actions$.pipe(
         ofType(BlocksActions.loadBlock),
         switchMap(action => {
            return this.blocksService.block(action.id.toString(), false, true).pipe(
               map(block => BlocksActions.blockLoaded({ entity: block })),
               catchError(error => of(BlocksActions.blockLoadError({ error: error })))
            );
         })
      )
   );

   loadBlocks$ = createEffect(() =>
      this.actions$.pipe(
         ofType(BlocksActions.loadBlocks),
         switchMap(action => {
            return this.blocksService.blocks(action.records).pipe(
               map(blocks => BlocksActions.blocksLoaded({ entities: blocks })),
               catchError(error => of(BlocksActions.blocksLoadError({ error: error })))
            );
         })
      )
   );

   loadStats$ = createEffect(() =>
      this.actions$.pipe(
         ofType(BlocksActions.loadStats),
         switchMap(action => {
            return this.blocksService.stats().pipe(
               map(stats => BlocksActions.statsLoaded({ stats: stats })),
               catchError(error => of(BlocksActions.loadStatsError({ error: error })))
            );
         })
      )
   );
}
