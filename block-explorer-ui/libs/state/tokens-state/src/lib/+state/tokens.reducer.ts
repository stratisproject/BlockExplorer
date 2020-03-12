import { TokensAction, TokensActionTypes } from './tokens.actions';

export const TOKENS_FEATURE_KEY = 'tokens';

/**
 * Interface for the 'Tokens' data used in
 *  - TokensState, and
 *  - tokensReducer
 *
 *  Note: replace if already defined in another module
 */

/* tslint:disable:no-empty-interface */
export interface Entity {
};

export interface TokensState {
  list        : Entity[];             // list of Tokens; analogous to a sql normalized table
  selectedId ?: string | number;      // which Tokens record has been selected
  loaded      : boolean;              // has the Tokens list been loaded
  error      ?: any;                  // last none error (if any)
};

export interface TokensPartialState {
  readonly [TOKENS_FEATURE_KEY]: TokensState;
}

export const initialState: TokensState = {
  list : [ ],
  loaded : false
};

export function tokensReducer(
  state: TokensState = initialState, 
  action: TokensAction): TokensState
{
  switch (action.type) {
    case TokensActionTypes.TokensLoaded: {
      state = {
        ...state,
        list  : action.payload,
        loaded: true
      };
      break;
    }
  }
  return state;
}
