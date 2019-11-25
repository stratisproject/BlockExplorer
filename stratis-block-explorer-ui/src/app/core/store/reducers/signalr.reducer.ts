import * as fromCore from '../actions/core.actions';
import { createReducer, on, Action } from '@ngrx/store';

export interface SignalrState {
    signalRConnectionEstablished: boolean;
    pending: boolean;
}

export const coreFeatureKey = 'core';

export const initialState: SignalrState = {
    signalRConnectionEstablished: false,
    pending: false
}

const reducer = createReducer(
    initialState,

    on(fromCore.signalrEstablishedConnection, state => state = ({
        ...state,
        signalRConnectionEstablished: true,
    })),

    on(fromCore.signalrFailedConnection, state => state = ({
        ...state,
        signalRConnectionEstablished: false
    }))
);

export const getPending = (state: SignalrState) => state.pending;

export function signalrReducer(state: SignalrState | undefined, action: Action) {
    return reducer(state, action);
}
