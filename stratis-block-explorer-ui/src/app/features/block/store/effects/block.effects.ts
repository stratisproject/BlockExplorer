import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, switchMap, tap } from 'rxjs/operators';
import { of } from 'rxjs';

import * as BlockActions from '../actions/block.actions';
import { BlocksService } from '../../services/blocks.service';


@Injectable()
export class BlockEffects {

    constructor(private actions$: Actions, private blocksService: BlocksService) { }

    loadLastBlocks$ = createEffect(() =>
        this.actions$.pipe(
            ofType(BlockActions.loadLastBlocks),
            switchMap(action => {
                return this.blocksService.blocks(action.records).pipe(
                    map(blocks => BlockActions.lastBlocksLoaded({ blocks: blocks })),
                    catchError(error => of(BlockActions.loadLastBlocksError({ error: error.toString() })))
                );
            })
        )
    );

    loadBlocks$ = createEffect(() =>
        this.actions$.pipe(
            ofType(BlockActions.loadBlocks),
            switchMap(action => {
                return this.blocksService.blocks(action.records).pipe(
                    map(blocks => BlockActions.blocksLoaded({ blocks: blocks })),
                    catchError(error => of(BlockActions.loadBlocksError({ error: error.toString() })))
                );
            })
        )
    );

    loadBlock$ = createEffect(() =>
        this.actions$.pipe(
            ofType(BlockActions.loadBlock),
            switchMap(action => {
                return this.blocksService.block(action.blockHash, false, true).pipe(
                    map(block => BlockActions.blockLoaded({ block: block })),
                    catchError(error => of(BlockActions.loadBlockError({ error: (<Error>error).message })))
                );
            })
        )
    );
}
