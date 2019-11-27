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

    showSuccess(text: string) {
        this.store.dispatch(fromActions.showSuccess(text));
    }

    showInformation(text: string) {
        this.store.dispatch(fromActions.showInformation(text));
    }

    showWarning(text: string) {
        this.store.dispatch(fromActions.showWarning(text));
    }

    showError(text: string, error?: Error | any) {
        this.store.dispatch(fromActions.showError(text, error));
    }
}
