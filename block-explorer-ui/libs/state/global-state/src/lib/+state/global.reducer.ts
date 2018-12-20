import { GlobalAction, GlobalActionTypes } from './global.actions';

export const GLOBAL_FEATURE_KEY = 'global';

/**
 * Interface for the 'Global' data used in
 *  - GlobalState, and
 *  - globalReducer
 *
 *  Note: replace if already defined in another module
 */

/* tslint:disable:no-empty-interface */
export interface Entity {}

export interface GlobalState {
  loaded: boolean;
  error?: any;
  identifiedType: 'PUBKEY_ADDRESS' | 'TRANSACTION';
  identifiedEntity: any;
}

export interface GlobalPartialState {
  readonly [GLOBAL_FEATURE_KEY]: GlobalState;
}

export const initialState: GlobalState = {
  loaded: false,
  identifiedType: undefined,
  identifiedEntity: undefined
};

export function globalReducer(
  state: GlobalState = initialState,
  action: GlobalAction
): GlobalState {
  switch (action.type) {
    case GlobalActionTypes.IndentifyEntity: {
      state = {
        ...state,
        loaded: false,
        identifiedEntity: undefined,
      };
      break;
    }
    case GlobalActionTypes.Identified: {
      state = {
        ...state,
        identifiedEntity: action.payload,
        identifiedType: !!action.payload ? action.payload.type : null,
        loaded: true
      };
      break;
    }
    case GlobalActionTypes.IdentificationError: {
      state = {
        ...state,
        identifiedEntity: null,
        identifiedType: null,
        loaded: true
      };
      break;
    }
  }
  return state;
}
