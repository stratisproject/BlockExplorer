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

const getIdentifiedEntity = createSelector(
  getGlobalState,
  getLoaded,
  (state: GlobalState, isLoaded) => {
    return isLoaded ? state.identifiedEntity : null;
  }
);
const getIdentifiedType = createSelector(
  getGlobalState,
  getLoaded,
  (state: GlobalState, isLoaded) => {
    return isLoaded ? state.identifiedType : null;
  }
);

export const globalQuery = {
  getLoaded,
  getError,
  getIdentifiedEntity,
  getIdentifiedType
};
