import { createFeatureSelector, createSelector } from '@ngrx/store';
import {
  SMARTCONTRACTS_FEATURE_KEY,
  SmartContractsState
} from './smart-contracts.reducer';

// Lookup the 'SmartContracts' feature state managed by NgRx
const getSmartContractsState = createFeatureSelector<SmartContractsState>(
  SMARTCONTRACTS_FEATURE_KEY
);

const getLoaded = createSelector(
  getSmartContractsState,
  (state: SmartContractsState) => state.loaded
);
const getError = createSelector(
  getSmartContractsState,
  (state: SmartContractsState) => state.error
);

const getAllSmartContracts = createSelector(
  getSmartContractsState,
  getLoaded,
  (state: SmartContractsState, isLoaded) => {
    return isLoaded ? state.list : [];
  }
);
const getSelectedId = createSelector(
  getSmartContractsState,
  (state: SmartContractsState) => state.selectedId
);
const getSelectedSmartContracts = createSelector(
  getAllSmartContracts,
  getSelectedId,
  (smartContracts, id) => {
    const result = smartContracts.find(it => it['id'] === id);
    return result ? Object.assign({}, result) : undefined;
  }
);

export const smartContractsQuery = {
  getLoaded,
  getError,
  getAllSmartContracts,
  getSelectedSmartContracts
};
