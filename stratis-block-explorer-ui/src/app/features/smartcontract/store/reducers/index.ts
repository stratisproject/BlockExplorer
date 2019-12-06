import { ActionReducerMap, Action } from '@ngrx/store';
import * as fromSelectedSmartContract from './selected-smartcontract-action.reducer';
import * as fromToken from './tokens.reducer';

export const smartContractFeatureKey = 'smartContract';

export interface SmartContractState {
    tokens: fromToken.TokenState,
    selectedSmartContractAction: fromSelectedSmartContract.SelectedSmartContractActionState
}

export const reducers: ActionReducerMap<SmartContractState> = {
    tokens: fromToken.reducer,
    selectedSmartContractAction: fromSelectedSmartContract.reducer
};
