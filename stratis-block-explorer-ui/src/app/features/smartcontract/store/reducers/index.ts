import { ActionReducerMap, Action } from '@ngrx/store';
import * as fromSelectedSmartContract from './selected-smartcontract-action.reducer';
import * as fromToken from './standard-tokens.reducer';

export const smartContractFeatureKey = 'smartContract';

export interface SmartContractState {
    standardTokens: fromToken.StandardTokenState,
    selectedSmartContractAction: fromSelectedSmartContract.SelectedSmartContractActionState
}

export const reducers: ActionReducerMap<SmartContractState> = {
    standardTokens: fromToken.reducer,
    selectedSmartContractAction: fromSelectedSmartContract.reducer
};
