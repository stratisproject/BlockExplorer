import { createFeatureSelector, createSelector } from '@ngrx/store';
import { BlockState, blockFeatureKey } from '../reducers';

export const selectBlockState = createFeatureSelector<BlockState>(blockFeatureKey);
const selectSelectedBlockState = createSelector(selectBlockState, (state: BlockState) => state.selectedBlock);


export const getSelectedBlock = createSelector(
    selectSelectedBlockState,
    (state) => state.block
);

export const getIsSelectedBlockLoaded = createSelector(
    selectSelectedBlockState,
    (state) => state.loaded
);

export const getSelectedBlockError$ = createSelector(
    selectSelectedBlockState,
    (state) => state.error
);

