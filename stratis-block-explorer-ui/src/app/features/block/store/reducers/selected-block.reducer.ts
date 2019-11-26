import { Action, createReducer, on } from '@ngrx/store';
import * as BlockActions from '../actions/block.actions';
import { BlockResponseModel } from '../../models/block-response.model';

export interface SelectedBlockState {
    block: BlockResponseModel,
    isSelected,
    error: Error | string
}

export const initialState: SelectedBlockState = {
    block: null,
    isSelected: false,
    error: null
};

const selectedBlockReducer = createReducer(
    initialState,

    on(BlockActions.loadBlock, state => state = ({
        ...state,
        isSelected: false,
        error: null
    })),

    on(BlockActions.blockLoaded, (state, action) => ({
        ...state,
        isSelected: true,
        block: action.block,
        error: null
    })),

    on(BlockActions.loadBlockError, (state, action) => ({
        ...state,
        isSelected: false,
        block: null,
        error: action.error
    }))
);

export function reducer(state: SelectedBlockState | undefined, action: Action) {
    return selectedBlockReducer(state, action);
}
