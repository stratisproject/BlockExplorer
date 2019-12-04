import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, switchMap } from 'rxjs/operators';
import { of } from 'rxjs';

import * as SelectedBlockActions from '../actions/selected-block.actions';
import { BlocksService } from '../../services/blocks.service';


@Injectable()
export class SelectedBlockEffects {

    constructor(private actions$: Actions, private blocksService: BlocksService) { }

    loadBlock$ = createEffect(() =>
        this.actions$.pipe(
            ofType(SelectedBlockActions.loadBlock),
            switchMap(action => {
                return this.blocksService.block(action.blockHash, false, true).pipe(
                    map(block => SelectedBlockActions.blockLoaded({ block: block })),
                    catchError(error => of(SelectedBlockActions.loadBlockError({ error: (<Error>error).message })))
                );
            })
        )
    );
}
