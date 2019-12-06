import { createFeatureSelector, createSelector } from '@ngrx/store';
import { SmartContractState, smartContractFeatureKey } from '../reducers';
import { MultipleEntitySelectorsHelper } from '@shared/ngrx';

const selectFeature = createFeatureSelector<SmartContractState>(smartContractFeatureKey);
const selectSelectedSmartContractAction = createSelector(selectFeature, (state: SmartContractState) => state.tokens);

const selectorHelper = new MultipleEntitySelectorsHelper(selectSelectedSmartContractAction);

export const getToken$ = selectorHelper.getEntities;
export const getTokenError$ = selectorHelper.getError;
export const getTokenLoaded$ = selectorHelper.getLoaded;
