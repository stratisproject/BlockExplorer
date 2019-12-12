import { createFeatureSelector, createSelector } from '@ngrx/store';
import { SmartContractState, smartContractFeatureKey } from '../reducers';
import { SingleEntitySelectorsHelper } from '@shared/ngrx';
import { ISmartContractAction } from '../../models';
import { SelectedSmartContractActionState } from '../reducers/selected-smartcontract-action.reducer';

const selectFeature = createFeatureSelector<SmartContractState>(smartContractFeatureKey);
const selectSelectedSmartContractAction = createSelector(selectFeature, (state: SmartContractState) => state.selectedSmartContractAction);

const selectorHelper = new SingleEntitySelectorsHelper<ISmartContractAction, SelectedSmartContractActionState>(selectSelectedSmartContractAction);

export const getSelectedSmartContractAction$ = selectorHelper.getEntity;
export const getSelectedSmartContractActionError$ = selectorHelper.getError;
export const getSelectedSmartContractActionLoaded$ = selectorHelper.getLoaded;
