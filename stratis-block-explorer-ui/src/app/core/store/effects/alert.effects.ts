import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { tap, take } from 'rxjs/operators';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import * as AlertActions from '../actions/alert.actions';


@Injectable()
export class AlertEffects {

    constructor(private actions$: Actions, private snackBar: MatSnackBar) { }

    showSuccess$ = createEffect(() =>
        this.actions$.pipe(
            ofType(AlertActions.showSuccess),
            tap(action => {
                this.openSnackbar(action.text, "Success", { duration: action.duration, panelClass: "alert-success" }, action.onDismiss);
            })
        ), { dispatch: false }
    );

    showInformation$ = createEffect(() =>
        this.actions$.pipe(
            ofType(AlertActions.showInformation),
            tap(action => {
                this.openSnackbar(action.text, "Info", { duration: action.duration, panelClass: "alert-info" }, action.onDismiss);
            })
        ), { dispatch: false }
    );

    showWarning$ = createEffect(() =>
        this.actions$.pipe(
            ofType(AlertActions.showWarning),
            tap(action => {
                this.openSnackbar(action.text, "Warning", { duration: action.duration, panelClass: "alert-warning" }, action.onDismiss);
            })
        ), { dispatch: false }
    );

    showError$ = createEffect(() =>
        this.actions$.pipe(
            ofType(AlertActions.showError),
            tap(action => {
                this.openSnackbar(action.text, "Error", { duration: action.duration, panelClass: "alert-error" }, action.onDismiss);
            })
        ), { dispatch: false }
    );


    openSnackbar(text: string, action: string, config: MatSnackBarConfig<any>, onDismiss: () => void) {
        let ref = this.snackBar.open(text, action, config);
        if (onDismiss != null) {
            ref.afterDismissed()
                .pipe(take(1))
                .subscribe(() => onDismiss());
        }
    }
}
