import {
   ActionReducer,
   ActionReducerMap,
   createFeatureSelector,
   createSelector,
   MetaReducer
} from '@ngrx/store';
import { environment } from '../../environments/environment';
import * as fromMain from '../features/main/store/reducers/main.reducer';
import * as fromRouter from '@ngrx/router-store';

export const appStoreFeatureKey = 'appStore';

export interface State {
   router: fromRouter.RouterReducerState,
   main: fromMain.MainState
}

export const reducers: ActionReducerMap<State> = {
   router: fromRouter.routerReducer,
   main: fromMain.reducer
};


export const metaReducers: MetaReducer<State>[] = !environment.production ? [] : [];
