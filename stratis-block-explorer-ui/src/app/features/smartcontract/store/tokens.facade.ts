import { Injectable } from '@angular/core';
import { select, Store } from '@ngrx/store';
import * as fromActions from './actions/tokens.actions';
import * as fromSelectors from './selectors/tokens.selectors';
import { TokenState } from './reducers/tokens.reducer';

@Injectable({ providedIn: 'root' })
export class SelectedSmartContractActionFacade {
    loaded$ = this.store.pipe(select(fromSelectors.getTokenLoaded$));
    error$ = this.store.pipe(select(fromSelectors.getTokenError$));
    token$ = this.store.pipe(select(fromSelectors.getToken$));

    constructor(private store: Store<TokenState>) { }

    load(from: number = 0, records: number = 10) {
        this.store.dispatch(fromActions.load(from, records));
    }
}
