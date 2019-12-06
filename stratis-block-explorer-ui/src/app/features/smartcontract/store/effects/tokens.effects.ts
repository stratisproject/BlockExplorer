import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, switchMap } from 'rxjs/operators';
import { of } from 'rxjs';

import * as TokensAction from '../actions/tokens.actions';
import { SmartContractService } from '../../services/smartcontract.service';


@Injectable()
export class TokensEffects {

    constructor(private actions$: Actions, private smartContractService: SmartContractService) { }

    loadTokens$ = createEffect(() =>
        this.actions$.pipe(
            ofType(TokensAction.load),
            switchMap(action => {
                return this.smartContractService.getTokens(action.from, action.records).pipe(
                    map(tokens => TokensAction.loaded({ entities: tokens })),
                    catchError(error => of(TokensAction.loadError({ error: error.toString() })))
                );
            })
        )
    );
}
