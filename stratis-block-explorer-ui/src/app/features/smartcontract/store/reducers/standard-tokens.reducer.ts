import { Action, createReducer, on } from '@ngrx/store';
import * as Actions from '../actions/standard-tokens.actions';
import * as fromModels from '../../models';
import { EntityState, EntityReducersHelper } from '@shared/ngrx';

export interface StandardTokenState extends EntityState<fromModels.StandardToken> { }

let reducerHelper = new EntityReducersHelper<fromModels.StandardToken, StandardTokenState>();

const innerReducer = createReducer(
    reducerHelper.getInitialState(),

    ...reducerHelper.getDefaultReducers(Actions.standardTokenActionHelper)
);

export function reducer(state: StandardTokenState | undefined, action: Action) {
    return innerReducer(state, action);
}
