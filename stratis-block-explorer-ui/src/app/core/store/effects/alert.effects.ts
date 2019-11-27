import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { tap } from 'rxjs/operators';
import { MatSnackBar } from '@angular/material/snack-bar';
import * as AlertActions from '../actions/alert.actions';


@Injectable()
export class AlertEffects {

    constructor(private actions$: Actions, private snackBar: MatSnackBar) { }

    showSuccess$ = createEffect(() =>
        this.actions$.pipe(
            ofType(AlertActions.showSuccess),
            tap(action => {
                this.snackBar.open(action.text, "Success", { duration: action.duration, panelClass: "alert-success" });
            })
        ), { dispatch: false }
    );

    showInformation$ = createEffect(() =>
        this.actions$.pipe(
            ofType(AlertActions.showInformation),
            tap(action => {
                this.snackBar.open(action.text, "Info", { duration: action.duration, panelClass: "alert-info" });
            })
        ), { dispatch: false }
    );

    showWarning$ = createEffect(() =>
        this.actions$.pipe(
            ofType(AlertActions.showWarning),
            tap(action => {
                this.snackBar.open(action.text, "Warning", { duration: action.duration, panelClass: "alert-warning" });
            })
        ), { dispatch: false }
    );

    showError$ = createEffect(() =>
        this.actions$.pipe(
            ofType(AlertActions.showError),
            tap(action => {
                this.snackBar.open(action.text, "Error", { duration: action.duration, panelClass: "alert-error" });
            })
        ), { dispatch: false }
    );
}
