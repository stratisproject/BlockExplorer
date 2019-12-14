import { EntityActionsHelper } from '@shared/ngrx';
import { BlockResponseModel } from '../../models/block-response.model';
import { createAction, props } from '@ngrx/store';
import { StatsModel } from '../../models';

export const blockActionHelper = new EntityActionsHelper<BlockResponseModel>("Block");

export const loadBlock = blockActionHelper.loadEntityAction;
export const blockLoadError = blockActionHelper.entityLoadErrorAction;
export const blockLoaded = blockActionHelper.entityLoadedAction;

export const loadBlocks = blockActionHelper.loadEntitiesAction;
export const blocksLoadError = blockActionHelper.entitiesLoadErrorAction;
export const blocksLoaded = blockActionHelper.entitiesLoadedAction;

export const loadStats = createAction(
   "[Blocks Stats] Load Entity"
);;
export const loadStatsError = createAction(
   "[Blocks Stats] Entity Load Error",
   props<{ error: Error | string }>()
);
export const statsLoaded = createAction(
   "[Blocks Stats] Entity Loaded",
   props<{ stats: StatsModel }>()
);
