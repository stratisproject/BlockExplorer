import { Action, createReducer, on } from '@ngrx/store';
import * as SelectedBlockActions from '../actions/selected-block.actions';
import { BlockResponseModel } from '../../models/block-response.model';

export interface SelectedBlockState {
    block: BlockResponseModel,
    loaded,
    error: Error | string
}

export const initialState: SelectedBlockState = {
    block: null,
    loaded: false,
    error: null
};

const selectedBlockReducer = createReducer(
    initialState,

    on(SelectedBlockActions.loadBlock, state => state = ({
        ...state,
        block: null,
        loaded: false,
        error: null
    })),

    on(SelectedBlockActions.blockLoaded, (state, action) => ({
        ...state,
        block: action.block,
        loaded: true,
        error: null
    })),

    on(SelectedBlockActions.loadBlockError, (state, action) => ({
        ...state,
        block: null,
        loaded: false,
        error: action.error
    }))
);

export function reducer(state: SelectedBlockState | undefined, action: Action) {
    return selectedBlockReducer(state, action);
}
