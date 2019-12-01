import { Injectable } from '@angular/core';
import { select, Store } from '@ngrx/store';
import * as fromActions from './actions';
import * as fromReducers from './reducers';
import * as fromSelectors from './selectors';

@Injectable({ providedIn: 'root' })
export class TransactionStoreFacade {
    isSelectedTransactionLoaded$ = this.store.pipe(select(fromSelectors.getIsSelectedTransactionLoaded));
    selectedTransactionError$ = this.store.pipe(select(fromSelectors.getSelectedTransactionError$));
    selectedTransaction$ = this.store.pipe(select(fromSelectors.getSelectedTransaction));

    constructor(private store: Store<fromReducers.TransactionState>) { }

    loadTransaction(txId: string) {
        this.store.dispatch(fromActions.loadTransaction(txId));
    }
}
