import { createFeatureSelector, createSelector } from '@ngrx/store';
import { GLOBAL_FEATURE_KEY, GlobalState } from './global.reducer';

// Lookup the 'Global' feature state managed by NgRx
const getGlobalState = createFeatureSelector<GlobalState>(GLOBAL_FEATURE_KEY);

const getLoaded = createSelector(
  getGlobalState,
  (state: GlobalState) => state.loaded
);
const getError = createSelector(
  getGlobalState,
  (state: GlobalState) => state.error
);

const getAllGlobal = createSelector(
  getGlobalState,
  getLoaded,
  (state: GlobalState, isLoaded) => {
    return isLoaded ? state.list : [];
  }
);
const getSelectedId = createSelector(
  getGlobalState,
  (state: GlobalState) => state.selectedId
);
const getSelectedGlobal = createSelector(
  getAllGlobal,
  getSelectedId,
  (global, id) => {
    const result = global.find(it => it['id'] === id);
    return result ? Object.assign({}, result) : undefined;
  }
);

export const globalQuery = {
  getLoaded,
  getError,
  getAllGlobal,
  getSelectedGlobal
};
