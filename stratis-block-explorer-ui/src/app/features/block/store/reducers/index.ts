import { ActionReducerMap } from '@ngrx/store';
import * as fromBlocks from './blocks.reducer';
import * as fromLastBlock from './last-blocks.reducer';

export const blockFeatureKey = 'block';

export interface BlockState {
    blocks: fromBlocks.BlocksState,
    lastBlock: fromLastBlock.LastBlocksState
}

export const reducers: ActionReducerMap<BlockState> = {
    blocks: fromBlocks.reducer,
    lastBlock: fromLastBlock.reducer
};
