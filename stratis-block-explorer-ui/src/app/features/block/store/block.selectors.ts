import { createFeatureSelector, createSelector } from '@ngrx/store';
import * as fromBlock from './block.reducer';

export const selectBlockState = createFeatureSelector<fromBlock.BlockState>(
  fromBlock.blockFeatureKey
);
