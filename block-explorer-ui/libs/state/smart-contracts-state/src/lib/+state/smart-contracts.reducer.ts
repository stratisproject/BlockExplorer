import {
  SmartContractsAction,
  SmartContractsActionTypes
} from './smart-contracts.actions';

export const SMARTCONTRACTS_FEATURE_KEY = 'smartContracts';

/**
 * Interface for the 'SmartContracts' data used in
 *  - SmartContractsState, and
 *  - smartContractsReducer
 *
 *  Note: replace if already defined in another module
 */

/* tslint:disable:no-empty-interface */
export interface Entity {}

export interface SmartContractsState {
  list: Entity[]; // list of SmartContracts; analogous to a sql normalized table
  selectedId?: string | number; // which SmartContracts record has been selected
  loaded: boolean; // has the SmartContracts list been loaded
  error?: any; // last none error (if any)
}

export interface SmartContractsPartialState {
  readonly [SMARTCONTRACTS_FEATURE_KEY]: SmartContractsState;
}

export const initialState: SmartContractsState = {
  list: [],
  loaded: false
};

export function smartContractsReducer(
  state: SmartContractsState = initialState,
  action: SmartContractsAction
): SmartContractsState {
  switch (action.type) {
    case SmartContractsActionTypes.SmartContractsLoaded: {
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
