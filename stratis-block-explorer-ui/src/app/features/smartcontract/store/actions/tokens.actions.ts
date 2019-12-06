import { createAction, props } from '@ngrx/store';
import { MultipleEntitiesActionsHelper } from '@shared/ngrx';
import * as fromModels from '../../models';

export const actionHelper = new MultipleEntitiesActionsHelper<fromModels.Token>("Token");

export const load = actionHelper.loadAction;
export const loadError = actionHelper.loadErrorAction;
export const loaded = actionHelper.loadedAction;
