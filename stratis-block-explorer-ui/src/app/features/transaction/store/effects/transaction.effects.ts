import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, switchMap } from 'rxjs/operators';
import { of } from 'rxjs';

import * as TransactionActions from '../actions/transaction.actions';
import { TransactionService } from '../../services/transaction.service';


@Injectable()
export class TransactionEffects {

    constructor(private actions$: Actions, private transactionService: TransactionService) { }

    loadTransaction$ = createEffect(() =>
        this.actions$.pipe(
            ofType(TransactionActions.loadTransaction),
            switchMap(action => {
                return this.transactionService.transaction(action.txId, false, true).pipe(
                    map(transaction => TransactionActions.transactionLoaded({ transaction: transaction })),
                    catchError(error => of(TransactionActions.loadTransactionError({ error: error })))
                );
            })
        )
    );
}
