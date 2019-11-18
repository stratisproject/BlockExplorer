import {
  ActionReducer,
  ActionReducerMap,
  createFeatureSelector,
  createSelector,
  MetaReducer
} from '@ngrx/store';
import { environment } from '../../environments/environment';
import * as fromHome from '../features/home/store/reducers/home.reducer';

export const appStoreFeatureKey = 'appStore';

export interface State {
  home: fromHome.HomeState
}

export const reducers: ActionReducerMap<State> = {
  home: fromHome.reducer
};


export const metaReducers: MetaReducer<State>[] = !environment.production ? [] : [];
