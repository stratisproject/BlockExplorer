import { Action, createReducer, on } from '@ngrx/store';
import * as BlockActions from '../actions/block.actions';
import { BlockResponseModel } from '../../models/block-response.model';

export const blockFeatureKey = 'block';

export interface LastBlocksState {
    blocks: BlockResponseModel[],
    areLoaded: boolean,
    lastBlocksError: Error | string
}

export const initialState: LastBlocksState = {
    blocks: [],
    areLoaded: false,
    lastBlocksError: null
};

const lastBlocksReducer = createReducer(
    initialState,

    on(BlockActions.loadLastBlocks, state => state = ({
        ...state,
        areLoaded: false,
    })),

    on(BlockActions.lastBlocksLoaded, (state, action) => state = ({
        ...state,
        areLoaded: true,
        blocks: action.blocks,
    })),

    on(BlockActions.loadLastBlocksError, (state, action) => state = ({
        ...state,
        areLoaded: true,
        blocks: null,
    }))
);

export function reducer(state: LastBlocksState | undefined, action: Action) {
    return lastBlocksReducer(state, action);
}
