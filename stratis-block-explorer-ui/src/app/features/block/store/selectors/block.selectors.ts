import { createFeatureSelector, createSelector } from '@ngrx/store';
import * as fromBlock from '../reducers';

export const selectBlockState = createFeatureSelector<fromBlock.BlockState>(
    fromBlock.blockFeatureKey
);

export const getLastBlocks = createSelector(selectBlockState, (state: fromBlock.BlockState) =>
    state.lastBlock.blocks
);

export const getAreLastBlocksLoaded = createSelector(selectBlockState, (state: fromBlock.BlockState) =>
    state.lastBlock.areLoaded
);

export const getBlocks = createSelector(selectBlockState, (state: fromBlock.BlockState) =>
    state.blocks.blocks
);

export const getAreBlocksLoaded = createSelector(selectBlockState, (state: fromBlock.BlockState) =>
    state.blocks.areLoaded
);

export const getSelectedBlock = createSelector(selectBlockState, (state: fromBlock.BlockState) =>
    state.selectedBlock.block
);

export const getIsSelectedBlockLoaded = createSelector(selectBlockState, (state: fromBlock.BlockState) =>
    state.selectedBlock.isSelected
);

export const getSelectedBlockError$ = createSelector(selectBlockState, (state: fromBlock.BlockState) =>
    state.selectedBlock.error
);

