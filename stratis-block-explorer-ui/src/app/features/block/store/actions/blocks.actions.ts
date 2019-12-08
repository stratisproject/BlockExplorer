import { EntityActionsHelper } from '@shared/ngrx';
import { BlockResponseModel } from '../../models/block-response.model';

export const blockActionHelper = new EntityActionsHelper<BlockResponseModel>("Block");

export const loadBlock = blockActionHelper.loadEntityAction;
export const blockLoadError = blockActionHelper.entityLoadErrorAction;
export const blockLoaded = blockActionHelper.entityLoadedAction;

export const loadBlocks = blockActionHelper.loadEntitiesAction;
export const blocksLoadError = blockActionHelper.entitiesLoadErrorAction;
export const blocksLoaded = blockActionHelper.entitiesLoadedAction;
