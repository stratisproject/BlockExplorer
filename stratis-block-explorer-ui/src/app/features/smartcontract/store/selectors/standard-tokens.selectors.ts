import { createFeatureSelector, createSelector } from '@ngrx/store';
import { SmartContractState, smartContractFeatureKey } from '../reducers';
import { MultipleEntitySelectorsHelper } from '@shared/ngrx';
import { StandardToken } from '../../models';
import { StandardTokenState } from '../reducers/standard-tokens.reducer';

const selectFeature = createFeatureSelector<SmartContractState>(smartContractFeatureKey);
const selectSelectedSmartContractAction = createSelector(selectFeature, (state: SmartContractState) => state.standardTokens);

const selectorHelper = new MultipleEntitySelectorsHelper<StandardToken, StandardTokenState>(selectSelectedSmartContractAction);

export const getStandardTokens$ = selectorHelper.getEntities;
export const getStandardTokensError$ = selectorHelper.getError;
export const getStandardTokensLoaded$ = selectorHelper.getLoaded;
