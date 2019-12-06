import { Injectable } from '@angular/core';
import { select, Store } from '@ngrx/store';
import * as fromActions from './actions/selected-smartcontract-action.actions';
import * as fromSelectors from './selectors/selected-smartcontract-action.selectors';
import { SelectedSmartContractActionState } from './reducers/selected-smartcontract-action.reducer';

@Injectable({ providedIn: 'root' })
export class SelectedSmartContractActionFacade {
    loaded$ = this.store.pipe(select(fromSelectors.getSelectedSmartContractActionLoaded$));
    error$ = this.store.pipe(select(fromSelectors.getSelectedSmartContractActionError$));
    smartContractActions$ = this.store.pipe(select(fromSelectors.getSelectedSmartContractActionLoaded$));

    constructor(private store: Store<SelectedSmartContractActionState>) { }

    load(id: string) {
        this.store.dispatch(fromActions.load(id));
    }
}
