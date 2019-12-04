import { createFeatureSelector, createSelector } from '@ngrx/store';
import { BlockState, blockFeatureKey } from '../reducers';

export const selectBlockState = createFeatureSelector<BlockState>(blockFeatureKey);
const selectLastBlocksState = createSelector(selectBlockState, (state: BlockState) => state.lastBlock);

export const getLastBlocks = createSelector(
    selectLastBlocksState,
    (state) => state.blocks
);

export const getAreLastBlocksLoaded = createSelector(
    selectLastBlocksState,
    (state) => state.loaded
);

export const loadLastBlocksError$ = createSelector(
    selectLastBlocksState,
    (state) => state.error
);
