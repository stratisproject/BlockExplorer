import { ActionReducer, ActionReducerMap, MetaReducer, Action, createFeatureSelector } from "@ngrx/store";
import * as fromRouter from "@ngrx/router-store";
import { InjectionToken } from "@angular/core";
import * as fromSignalr from './signalr.reducer';
import { environment } from '../../../../environments/environment';

export interface RootState {
    router: fromRouter.RouterReducerState<any>;
    signalr: fromSignalr.SignalrState;
}

const rootReducers: ActionReducerMap<RootState> = {
    router: fromRouter.routerReducer,
    signalr: fromSignalr.signalrReducer
}

export const ROOT_REDUCERS = new InjectionToken("Root reducers token", { factory: () => rootReducers });

// console.log all actions
export function logger(reducer: ActionReducer<RootState>): ActionReducer<RootState> {
    return (state, action) => {
        const result = reducer(state, action);
        console.groupCollapsed(action.type);
        console.log("prev state", state);
        console.log("action", action);
        console.log("next state", result);
        console.groupEnd();

        return result;
    };
}

export const metaReducers: MetaReducer<RootState>[] = !environment.production ? [logger] : [];
