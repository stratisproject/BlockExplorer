import { createAction, props } from '@ngrx/store';
import { MultipleEntitiesActionsHelper } from '@shared/ngrx';
import * as fromModels from '../../models';

export const standardTokenActionHelper = new MultipleEntitiesActionsHelper<fromModels.StandardToken>("StandardToken");

export const loadStandardTokens = standardTokenActionHelper.loadAction;
export const standardTokensLoadError = standardTokenActionHelper.loadErrorAction;
export const standardTokensLoaded = standardTokenActionHelper.loadedAction;
