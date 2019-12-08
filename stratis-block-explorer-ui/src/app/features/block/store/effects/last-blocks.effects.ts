import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, switchMap } from 'rxjs/operators';
import { of } from 'rxjs';

import * as LastBlockActions from '../actions/last-blocks.actions';
import { BlocksService } from '../../services/blocks.service';


@Injectable()
export class LastBlockEffects {

    constructor(private actions$: Actions, private blocksService: BlocksService) { }

    loadLastBlocks$ = createEffect(() =>
        this.actions$.pipe(
            ofType(LastBlockActions.loadLastBlocks),
            switchMap(action => {
                return this.blocksService.blocks(action.records).pipe(
                    map(blocks => LastBlockActions.lastBlocksLoaded({ blocks: blocks })),
                    catchError(error => of(LastBlockActions.loadLastBlocksError({ error: error })))
                );
            })
        )
    );
}
