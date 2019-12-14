import { createFeatureSelector, createSelector } from '@ngrx/store';
import { blockFeatureKey, BlockState } from '../reducers';
import { BlocksState } from '../reducers/blocks.reducer';
import { EntitySelectorsHelper } from '@shared/ngrx';
import { BlockResponseModel } from '../../models';

const selectFeature = createFeatureSelector<BlockState>(blockFeatureKey);
const selectState = createSelector(selectFeature, (state: BlockState) => state.blocks);

const selectorHelper = new EntitySelectorsHelper<BlockResponseModel, BlocksState>(selectState);

export const getBlock$ = selectorHelper.getEntity;
export const getBlockError$ = selectorHelper.getEntityLoadError;
export const getBlockLoaded$ = selectorHelper.getEntityLoaded;

export const getBlocks$ = selectorHelper.getEntities;
export const getBlocksError$ = selectorHelper.getEntitiesLoadError;
export const getBlocksLoaded$ = selectorHelper.getEntitiesLoaded;

export const getStats$ = createSelector(
   selectState,
   (state) => state.stats
);
export const getStatsError$ = createSelector(
   selectState,
   (state) => state.statsLoadError
);
export const getStatsLoaded$ = createSelector(
   selectState,
   (state) => state.statsLoaded
);
