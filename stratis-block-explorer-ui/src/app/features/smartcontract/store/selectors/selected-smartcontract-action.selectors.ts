import { createFeatureSelector, createSelector } from '@ngrx/store';
import { SmartContractState, smartContractFeatureKey } from '../reducers';
import { SingleEntitySelectorsHelper } from '@shared/ngrx';

const selectFeature = createFeatureSelector<SmartContractState>(smartContractFeatureKey);
const selectSelectedSmartContractAction = createSelector(selectFeature, (state: SmartContractState) => state.selectedSmartContractAction);

const selectorHelper = new SingleEntitySelectorsHelper(selectSelectedSmartContractAction);

export const getSelectedSmartContractAction$ = selectorHelper.getEntity;
export const getSelectedSmartContractActionError$ = selectorHelper.getError;
export const getSelectedSmartContractActionLoaded$ = selectorHelper.getLoaded;
