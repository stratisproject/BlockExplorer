import { createFeatureSelector, createSelector } from '@ngrx/store';
import * as fromHome from '../reducers/home.reducer';

export const selectHomeState = createFeatureSelector<fromHome.HomeState>(
   fromHome.homeFeatureKey
);

export const getLoaded = createSelector(
   selectHomeState,
   (state: fromHome.HomeState) => state.loaded
);

export const getError = createSelector(
   selectHomeState,
   (state: fromHome.HomeState) => state.error
);

export const getIdentifiedEntity = createSelector(
   selectHomeState,
   getLoaded,
   (state: fromHome.HomeState, isLoaded) => isLoaded ? state.identifiedEntity : undefined
);

export const getIdentifiedType = createSelector(
   selectHomeState,
   getLoaded,
   (state: fromHome.HomeState, isLoaded) => isLoaded ? state.identifiedType : null
);
