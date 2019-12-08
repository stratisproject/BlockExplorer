import { createAction, props } from '@ngrx/store';
import { EntityActionsHelper } from '@shared/ngrx';
import * as fromModels from '../../models';

export const standardTokenActionHelper = new EntityActionsHelper<fromModels.StandardToken>("StandardToken");

export const loadStandardToken = standardTokenActionHelper.loadEntityAction;
export const standardTokenLoadError = standardTokenActionHelper.entityLoadErrorAction;
export const standardTokenLoaded = standardTokenActionHelper.entityLoadedAction;

export const loadStandardTokens = standardTokenActionHelper.loadEntitiesAction;
export const standardTokensLoadError = standardTokenActionHelper.entitiesLoadErrorAction;
export const standardTokensLoaded = standardTokenActionHelper.entitiesLoadedAction;
