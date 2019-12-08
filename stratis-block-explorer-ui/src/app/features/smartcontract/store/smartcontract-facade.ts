import { Injectable } from '@angular/core';
import { select, Store } from '@ngrx/store';
import * as fromActions from './actions';
import * as fromSelectors from './selectors';
import { SmartContractState } from './reducers';
import { StandardTokenState } from './reducers/standard-tokens.reducer';
import { SelectedSmartContractActionState } from './reducers/selected-smartcontract-action.reducer';

@Injectable({ providedIn: 'root' })
export class SmartContractFacade {
    tokensLoaded$ = this.tokenStore.pipe(select(fromSelectors.getStandardTokensLoaded$));
    tokensError$ = this.tokenStore.pipe(select(fromSelectors.getStandardTokensError$));
    tokens$ = this.tokenStore.pipe(select(fromSelectors.getStandardTokens$));

    smartContractActionLoaded$ = this.selectedSmartContractActionStore.pipe(select(fromSelectors.getSelectedSmartContractActionLoaded$));
    smartContractActionError$ = this.selectedSmartContractActionStore.pipe(select(fromSelectors.getSelectedSmartContractActionError$));
    smartContractAction$ = this.selectedSmartContractActionStore.pipe(select(fromSelectors.getSelectedSmartContractActionLoaded$));

    constructor(private tokenStore: Store<StandardTokenState>, private selectedSmartContractActionStore: Store<SelectedSmartContractActionState>) { }

    getStandardTokens(from: number = 0, records: number = 10) {
        this.tokenStore.dispatch(fromActions.loadStandardTokens(from, records));
    }

    getStandardToken(contractAddress: string) {
        this.tokenStore.dispatch(fromActions.loadStandardToken(contractAddress));
    }

    getSmartContractAction(txId: string) {
        this.selectedSmartContractActionStore.dispatch(fromActions.loadSmartContractAction(txId));
    }
}
