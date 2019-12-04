import { Action, createReducer, on } from '@ngrx/store';
import * as BlocksActions from '../actions/blocks.actions';
import { BlockResponseModel } from '../../models/block-response.model';

export interface BlocksState {
    blocks: BlockResponseModel[],
    loaded: boolean,
    error: Error | string
}

export const initialState: BlocksState = {
    blocks: [],
    loaded: false,
    error: null
};

const blocksReducer = createReducer(
    initialState,

    on(BlocksActions.loadBlocks, state => state = ({
        ...state,
        blocks: [],
        loaded: false,
        error: null
    })),

    on(BlocksActions.blocksLoaded, (state, action) => state = ({
        ...state,
        blocks: action.blocks,
        loaded: true,
        error: null
    })),

    on(BlocksActions.loadBlocksError, (state, action) => state = ({
        ...state,
        loaded: true,
        blocks: null,
        error: action.error
    }))
);

export function reducer(state: BlocksState | undefined, action: Action) {
    return blocksReducer(state, action);
}
