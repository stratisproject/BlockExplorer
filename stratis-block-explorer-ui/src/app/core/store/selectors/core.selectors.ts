import { createSelector, createFeatureSelector } from '@ngrx/store';
import * as fromCore from '../reducers/signalr.reducer';

export const selectCoreState = createFeatureSelector<fromCore.SignalrState>(fromCore.coreFeatureKey);

export const getCompleteCoreState = createSelector(
    selectCoreState,
    (state: fromCore.SignalrState) => state
);

export const getPending = createSelector(
    getCompleteCoreState,
    fromCore.getPending
);
