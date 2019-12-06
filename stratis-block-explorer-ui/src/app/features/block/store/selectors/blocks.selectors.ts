import { createFeatureSelector, createSelector } from '@ngrx/store';
//import * as fromBlocks from '../reducers/blocks.reducer';
import { blockFeatureKey, BlockState } from '../reducers';

const selectBlockState = createFeatureSelector<BlockState>(blockFeatureKey);
const getBlocksState = createSelector(selectBlockState, (state: BlockState) => state.blocks);

export const getBlocks = createSelector(
    getBlocksState,
    (state) => state.blocks
);

export const getAreBlocksLoaded = createSelector(
    getBlocksState,
    (state) => state.loaded
);

export const getLoadBlocksError$ = createSelector(
    getBlocksState,
    (state) => state.error
);
