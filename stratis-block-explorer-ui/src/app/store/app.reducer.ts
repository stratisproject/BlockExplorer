//import { ActionReducerMap, MetaReducer } from '@ngrx/store';
//import { environment } from '../../environments/environment';
//import * as fromRouter from '@ngrx/router-store';
//import * as fromMain from '../features/main/store/main.reducer';
//import * as fromBlock from '../features/block/store/reducers';

//export const appStoreFeatureKey = 'appStore';

//export interface State {
//    router: fromRouter.RouterReducerState,
//    main: fromMain.MainState
//    block: fromBlock.BlockState
//}

//export const reducers: ActionReducerMap<State> = {
//    router: fromRouter.routerReducer,
//    main: fromMain.reducer,
//    block: fromBlock.reducers
//};

//export const metaReducers: MetaReducer<State>[] = !environment.production ? [] : [];
