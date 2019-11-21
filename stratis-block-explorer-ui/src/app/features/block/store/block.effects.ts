import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, switchMap } from 'rxjs/operators';
import { of } from 'rxjs';

import * as BlockActions from './block.actions';
import { BlocksService } from '../services/blocks.service';


@Injectable()
export class BlockEffects {

   constructor(private actions$: Actions, private blocksService: BlocksService) { }

   loadLastBlocks$ = createEffect(() =>
      this.actions$.pipe(
         ofType(BlockActions.loadLastBlocks),
         switchMap(action => {
            return this.blocksService.blocks(action.records).pipe(
               map(blocks => BlockActions.lastBlocksLoaded({ blocks: blocks })),
               catchError(error => of(BlockActions.lastBlocksLoadedError({ error: error.toString() })))
            );
         })
      )
   );
}
