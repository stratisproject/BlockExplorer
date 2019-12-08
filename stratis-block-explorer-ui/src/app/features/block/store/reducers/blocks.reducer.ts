import { Action, createReducer, on } from '@ngrx/store';
import * as Actions from '../actions/blocks.actions';
import * as fromModels from '../../models';
import { EntityState, EntityReducersHelper } from '@shared/ngrx';

export interface BlocksState extends EntityState<fromModels.BlockResponseModel> { }

let reducerHelper = new EntityReducersHelper<fromModels.BlockResponseModel, BlocksState>();

const innerReducer = createReducer(
    reducerHelper.getInitialState(),

    ...reducerHelper.getDefaultReducers(Actions.blockActionHelper)
);

export function reducer(state: BlocksState | undefined, action: Action) {
    return innerReducer(state, action);
}
