import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, switchMap } from 'rxjs/operators';
import { of } from 'rxjs';

import * as SmartContractActionActions from '../actions/selected-smartcontract-action.actions';
import { SmartContractService } from '../../services/smartcontract.service';


@Injectable()
export class SmartContractActionEffects {

    constructor(private actions$: Actions, private smartContractService: SmartContractService) { }

    loadTokens$ = createEffect(() =>
        this.actions$.pipe(
            ofType(SmartContractActionActions.loadSmartContractAction),
            switchMap(action => {
                return this.smartContractService.getSmartContractAction(action.id.toString()).pipe(
                    map(smartContractAction => SmartContractActionActions.smartContractActionLoaded({ entity: smartContractAction })),
                    catchError(error => of(SmartContractActionActions.smartContractActionLoadError({ error: error.toString() })))
                );
            })
        )
    );
}
