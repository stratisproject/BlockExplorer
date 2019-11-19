import { createFeatureSelector, createSelector } from '@ngrx/store';
import * as fromMain from '../reducers/main.reducer';

export const selectMainState = createFeatureSelector<fromMain.MainState>(
   fromMain.mainFeatureKey
);

export const getLoaded = createSelector(
   selectMainState,
   (state: fromMain.MainState) => state.loaded
);

export const getError = createSelector(
   selectMainState,
   (state: fromMain.MainState) => state.error
);

export const getIdentifiedEntity = createSelector(
   selectMainState,
   getLoaded,
   (state: fromMain.MainState, isLoaded) => isLoaded ? state.identifiedEntity : undefined
);

export const getIdentifiedType = createSelector(
   selectMainState,
   getLoaded,
   (state: fromMain.MainState, isLoaded) => isLoaded ? state.identifiedType : null
);
