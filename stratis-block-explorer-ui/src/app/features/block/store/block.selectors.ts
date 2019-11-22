import { createFeatureSelector, createSelector } from '@ngrx/store';
import * as fromBlock from './block.reducer';

export const selectBlockState = createFeatureSelector<fromBlock.BlockState>(
   fromBlock.blockFeatureKey
);

export const getLastBlocksLoaded = createSelector(
   selectBlockState,
   (state: fromBlock.BlockState) => state.lastBlocksLoaded
);

export const getLastBlocks = createSelector(
   selectBlockState,
   (state: fromBlock.BlockState) => state.lastBlocks
);
