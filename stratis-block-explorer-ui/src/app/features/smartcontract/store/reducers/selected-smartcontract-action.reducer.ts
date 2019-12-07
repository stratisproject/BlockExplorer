import { Action, createReducer, on } from '@ngrx/store';
import * as Actions from '../actions/selected-smartcontract-action.actions';
import * as fromModels from '../../models';
import { SingleEntityState, SingleEntityReducersHelper } from '@shared/ngrx';

export interface SelectedSmartContractActionState extends SingleEntityState<fromModels.SmartContractAction> { }

let reducerHelper = new SingleEntityReducersHelper<fromModels.SmartContractAction, SelectedSmartContractActionState>();

const selectedBlockReducer = createReducer(
    reducerHelper.getInitialState(),

    ...reducerHelper.getDefaultReducers(Actions.SmartContractActionActionHelper)

    //on(Actions.load, state => reducerHelper.applyLoadReducer(state)),
    //on(Actions.loaded, (state, action) => reducerHelper.applyLoadedReducer(state, action.entity)),
    //on(Actions.loadError, (state, action) => reducerHelper.applyLoadErrorReducer(state, action.error)),
);

export function reducer(state: SelectedSmartContractActionState | undefined, action: Action) {
    return selectedBlockReducer(state, action);
}
