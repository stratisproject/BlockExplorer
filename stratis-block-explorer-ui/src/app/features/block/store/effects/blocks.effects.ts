import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, switchMap } from 'rxjs/operators';
import { of } from 'rxjs';

import * as BlocksActions from '../actions/blocks.actions';
import { BlocksService } from '../../services/blocks.service';


@Injectable()
export class BlocksEffects {

    constructor(private actions$: Actions, private blocksService: BlocksService) { }

    loadBlocks$ = createEffect(() =>
        this.actions$.pipe(
            ofType(BlocksActions.loadBlocks),
            switchMap(action => {
                return this.blocksService.blocks(action.records).pipe(
                    map(blocks => BlocksActions.blocksLoaded({ blocks: blocks })),
                    catchError(error => of(BlocksActions.loadBlocksError({ error: error.toString() })))
                );
            })
        )
    );
}
