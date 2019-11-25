import { Action, createReducer, on } from '@ngrx/store';
import * as BlockActions from '../actions/block.actions';
import { BlockResponseModel } from '../../models/block-response.model';

export interface BlocksState {
    blocks: BlockResponseModel[],
    areLoaded: boolean
}

export const initialState: BlocksState = {
    blocks: [],
    areLoaded: false,
};

const blocksReducer = createReducer(
    initialState,

    on(BlockActions.loadLastBlocks, state => state = ({
        ...state,
        areLoaded: false,
    })),

    on(BlockActions.blocksLoaded, (state, action) => state = ({
        ...state,
        areLoaded: true,
        blocks: action.blocks,
    }))
);

export function reducer(state: BlocksState | undefined, action: Action) {
    return blocksReducer(state, action);
}
