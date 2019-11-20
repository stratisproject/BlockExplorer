import { Action, createReducer, on } from '@ngrx/store';
import * as MainActions from './main.actions';

export const mainFeatureKey = 'main';

export interface MainState {
   loaded: boolean;
   error?: Error | any;
   identifiedType: 'PUBKEY_ADDRESS' | 'TRANSACTION' | 'SMART_CONTRACT';
   identifiedEntity: any;
}

export const initialState: MainState = {
   loaded: true,
   identifiedType: undefined,
   identifiedEntity: undefined
};

const mainReducer = createReducer(
   initialState,

   on(MainActions.identified, (state, action) => ({
      ...state,
      identifiedEntity: action.payload,
      identifiedType: !!action.payload ? action.payload.type : null,
      loaded: true
   })),

   on(MainActions.identificationError, (state, action) => ({
      ...state,
      error: action.error,
      identifiedEntity: null,
      identifiedType: null,
      loaded: true
   })),

   on(MainActions.identifyEntity, (state, action) => ({
      ...state,
      error: null,
      loaded: false,
      identifiedEntity: undefined,
   }))
);

export function reducer(state: MainState | undefined, action: Action) {
   return mainReducer(state, action);
}
