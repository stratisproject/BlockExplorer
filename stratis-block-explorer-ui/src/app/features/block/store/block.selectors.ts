import { createFeatureSelector, createSelector } from '@ngrx/store';
import * as fromBlock from './block.reducer';

export const selectBlockState = createFeatureSelector<fromBlock.BlockState>(
    fromBlock.blockFeatureKey
);

export const getLastBlocks = createSelector(selectBlockState, (state: fromBlock.BlockState) =>
    state.lastBlocks
);

export const getAreLastBlocksLoaded = createSelector(selectBlockState, (state: fromBlock.BlockState) =>
    state.areLastBlocksLoaded
);

export const getBlocks = createSelector(selectBlockState, (state: fromBlock.BlockState) =>
    state.blocks
);

export const getAreBlocksLoaded = createSelector(selectBlockState, (state: fromBlock.BlockState) =>
    state.areBlocksLoaded
);

export const getSelectedBlock = createSelector(selectBlockState, (state: fromBlock.BlockState) =>
    state.selectedBlock
);

export const getIsSelectedBlockLoaded = createSelector(selectBlockState, (state: fromBlock.BlockState) =>
    state.isSelectedBlockLoaded
);


