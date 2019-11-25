import { createAction } from '@ngrx/store';

export const signalrEstablishConnection = createAction(
    '[Core] SignalR Establish Connection'
);

export const signalrEstablishedConnection = createAction(
    '[Core] SignalR Established Connection'
);

export const signalrFailedConnection = createAction(
    '[Core] SignalR Failed Connection'
);
