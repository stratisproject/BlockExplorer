import { createFeatureSelector, createSelector } from '@ngrx/store';
import { SmartContractState, smartContractFeatureKey } from '../reducers';
import { EntitySelectorsHelper } from '@shared/ngrx';
import { StandardToken } from '../../models';
import { StandardTokenState } from '../reducers/standard-tokens.reducer';

const selectFeature = createFeatureSelector<SmartContractState>(smartContractFeatureKey);
const selectState = createSelector(selectFeature, (state: SmartContractState) => state.standardTokens);

const selectorHelper = new EntitySelectorsHelper<StandardToken, StandardTokenState>(selectState);

export const getStandardToken$ = selectorHelper.getEntity;
export const getStandardTokenError$ = selectorHelper.getEntityLoadError;
export const getStandardTokenLoaded$ = selectorHelper.getEntityLoaded;

export const getStandardTokens$ = selectorHelper.getEntities;
export const getStandardTokensError$ = selectorHelper.getEntitiesLoadError;
export const getStandardTokensLoaded$ = selectorHelper.getEntitiesLoaded;
