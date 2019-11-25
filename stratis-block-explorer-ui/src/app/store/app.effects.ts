import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, switchMap, tap } from 'rxjs/operators';
import { of } from 'rxjs';

import * as AppActions from './app.actions';
import { MatSnackBar } from '@angular/material/snack-bar';


@Injectable()
export class AppEffects {

    constructor(private actions$: Actions, private snackBar: MatSnackBar) { }

    notifyMessage$ = createEffect(({ snackbar = this.snackBar } = {}) =>
        this.actions$.pipe(
            ofType(AppActions.showMessage),
            tap(action => snackbar.open(action.text, 'Message', { duration: 3000 }))
        )
    );

    notifyWarning$ = createEffect(({ snackbar = this.snackBar } = {}) =>
        this.actions$.pipe(
            ofType(AppActions.showWarning),
            tap(action => snackbar.open(action.text, 'Warning', { duration: 3000 }))
        )
    );

    notifyError$ = createEffect(({ snackbar = this.snackBar } = {}) =>
        this.actions$.pipe(
            ofType(AppActions.showError),
            tap(action => snackbar.open(`${action.text}`, 'Error', { duration: 3000 }))
        )
    );
}
