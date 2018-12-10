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
  list: Entity[]; // list of Global; analogous to a sql normalized table
  selectedId?: string | number; // which Global record has been selected
  loaded: boolean; // has the Global list been loaded
  error?: any; // last none error (if any)
  identifiedType: 'ADDRESS' | 'TRANSACTION';
}

export interface GlobalPartialState {
  readonly [GLOBAL_FEATURE_KEY]: GlobalState;
}

export const initialState: GlobalState = {
  list: [],
  loaded: false,
  identifiedType: null
};

export function globalReducer(
  state: GlobalState = initialState,
  action: GlobalAction
): GlobalState {
  switch (action.type) {
    case GlobalActionTypes.GlobalLoaded: {
      state = {
        ...state,
        list: action.payload,
        loaded: true
      };
      break;
    }
  }
  return state;
}
