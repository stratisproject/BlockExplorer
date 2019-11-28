import { Injectable } from '@angular/core';
import { select, Store } from '@ngrx/store';
import * as fromActions from './actions';
import * as fromReducers from './reducers';
import * as fromSelectors from './selectors';

@Injectable({ providedIn: 'root' })
export class CoreStoreFacade {
    loginPending$ = this.store.pipe(select(fromSelectors.getPending));

    constructor(private store: Store<fromReducers.RootState>) { }

    establishSignalRConnection() {
        this.store.dispatch(fromActions.signalrEstablishConnection());
    }

    showSuccess(text: string, onDismiss: () => void = null) {
        this.store.dispatch(fromActions.showSuccess(text, 2000, onDismiss));
    }

    showInformation(text: string, onDismiss: () => void = null) {
        this.store.dispatch(fromActions.showInformation(text, 2000, onDismiss));
    }

    showWarning(text: string, onDismiss: () => void = null) {
        this.store.dispatch(fromActions.showWarning(text, 2000, onDismiss));
    }

    showError(text: string, error?: Error | any, onDismiss: () => void = null) {
        this.store.dispatch(fromActions.showError(text, error, 2000, onDismiss));
    }
}
