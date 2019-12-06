import { createAction, props } from '@ngrx/store';
import { SingleEntityActionsHelper } from '@shared/ngrx';
import * as fromModels from '../../models';

export const actionHelper = new SingleEntityActionsHelper<fromModels.SmartContractAction>("Smart Contract Action");

export const load = actionHelper.loadAction;
export const loadError = actionHelper.loadErrorAction;
export const loaded = actionHelper.loadedAction;
