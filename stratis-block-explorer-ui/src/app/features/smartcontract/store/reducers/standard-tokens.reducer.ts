import { Action, createReducer, on } from '@ngrx/store';
import * as Actions from '../actions/standard-tokens.actions';
import * as fromModels from '../../models';
import { MultipleEntitiesState, MultipleEntitiesReducersHelper } from '@shared/ngrx';

export interface StandardTokenState extends MultipleEntitiesState<fromModels.StandardToken> { }

let reducerHelper = new MultipleEntitiesReducersHelper<fromModels.StandardToken, StandardTokenState>();

const innerReducer = createReducer(
    reducerHelper.getInitialState(),

    ...reducerHelper.getDefaultReducers(Actions.standardTokenActionHelper)
);

export function reducer(state: StandardTokenState | undefined, action: Action) {
    return innerReducer(state, action);
}
