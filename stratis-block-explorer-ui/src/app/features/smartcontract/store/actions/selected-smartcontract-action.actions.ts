import { createAction, props } from '@ngrx/store';
import { SingleEntityActionsHelper } from '@shared/ngrx';
import * as fromModels from '../../models';

export const SmartContractActionActionHelper = new SingleEntityActionsHelper<fromModels.SmartContractAction>("Smart Contract Action");

export const loadSmartContractAction = SmartContractActionActionHelper.loadAction;
export const smartContractActionLoadError = SmartContractActionActionHelper.loadErrorAction;
export const smartContractActionLoaded = SmartContractActionActionHelper.loadedAction;
