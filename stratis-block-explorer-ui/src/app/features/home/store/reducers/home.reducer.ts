import { Action, createReducer, on } from '@ngrx/store';
import * as HomeActions from '../actions/home.actions';

export const homeFeatureKey = 'home';

export interface HomeState {
  loaded: boolean;
  error?: Error | any;
  identifiedType: 'PUBKEY_ADDRESS' | 'TRANSACTION' | 'SMART_CONTRACT';
  identifiedEntity: any;
}

export const initialState: HomeState = {
  loaded: false,
  identifiedType: undefined,
  identifiedEntity: undefined
};

const homeReducer = createReducer(
  initialState,

  on(HomeActions.identified, (state, action) => ({
    ...state,
    identifiedEntity: action.payload,
    identifiedType: !!action.payload ? action.payload.type : null,
    loaded: true
  })),

  on(HomeActions.identificationError, (state, action) => ({
    ...state,
    //error: action.error,
    identifiedEntity: null,
    identifiedType: null,
    loaded: true
  })),

  on(HomeActions.identifyEntity, (state, action) => ({
    ...state,
    loaded: false,
    identifiedEntity: undefined,
  }))
);

export function reducer(state: HomeState | undefined, action: Action) {
  return homeReducer(state, action);
}
