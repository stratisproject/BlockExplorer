import { Action, createReducer, on } from '@ngrx/store';
import * as Actions from '../actions/tokens.actions';
import * as fromModels from '../../models';
import { MultipleEntitiesState, MultipleEntitiesReducersHelper } from '@shared/ngrx';

export interface TokenState extends MultipleEntitiesState<fromModels.Token> { }

let reducerHelper = new MultipleEntitiesReducersHelper<fromModels.Token>();

const innerReducer = createReducer(
    reducerHelper.getInitialState<TokenState>(),

    ...reducerHelper.getDefaultReducers(Actions.actionHelper)
);

export function reducer(state: TokenState | undefined, action: Action) {
    return innerReducer(state, action);
}
