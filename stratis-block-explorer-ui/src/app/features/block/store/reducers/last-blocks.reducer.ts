import { Action, createReducer, on } from '@ngrx/store';
import * as LastBlockActions from '../actions/last-blocks.actions';
import { BlockResponseModel } from '../../models/block-response.model';

export interface LastBlocksState {
    blocks: BlockResponseModel[],
    loaded: boolean,
    error: Error | string
}

export const initialState: LastBlocksState = {
    blocks: [],
    loaded: false,
    error: null
};

const lastBlocksReducer = createReducer(
    initialState,

    on(LastBlockActions.loadLastBlocks, state => state = ({
        ...state,
        blocks: [],
        loaded: false,
        error: null
    })),

    on(LastBlockActions.lastBlocksLoaded, (state, action) => state = ({
        ...state,
        blocks: action.blocks,
        loaded: true,
        error: null
    })),

    on(LastBlockActions.loadLastBlocksError, (state, action) => state = ({
        ...state,
        loaded: true,
        blocks: null,
        error: action.error
    }))
);

export function reducer(state: LastBlocksState | undefined, action: Action) {
    return lastBlocksReducer(state, action);
}
