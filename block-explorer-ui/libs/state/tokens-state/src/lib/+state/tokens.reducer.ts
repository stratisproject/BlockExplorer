import { TokensAction, TokensActionTypes } from './tokens.actions';
import { TokenTransactionResponse } from '../services/token-transaction-response';
import { TokenDetail } from '../services/token-detail';

export const TOKENS_FEATURE_KEY = 'tokens';

export interface TokensState {
  list        : TokenTransactionResponse[];             // list of Tokens; analogous to a sql normalized table
  selectedId ?: string | number;      // which Tokens record has been selected
  loaded      : boolean;              // has the Tokens list been loaded
  error      ?: any;                  // last none error (if any)

  tokenDetails?: TokenDetail;
  detailLoaded: boolean;
};

export interface TokensPartialState {
  readonly [TOKENS_FEATURE_KEY]: TokensState;
}

export const initialState: TokensState = {
  list : [ ],
  loaded : false,  
  detailLoaded: false
};

export function tokensReducer(
  state: TokensState = initialState, 
  action: TokensAction): TokensState
{
  switch (action.type) {
    case TokensActionTypes.LoadTokenDetail: {
      state = {
        ...state,
        detailLoaded: false
      }
      break;
    }

    case TokensActionTypes.TokenDetailLoaded: {
      state = {
        ...state,
        detailLoaded: true,
        tokenDetails: action.payload
      }
      break;
    }

    case TokensActionTypes.TokenDetailLoadError: {
      state = {
        ...state,
        detailLoaded: true
      }
      break;
    }

    case TokensActionTypes.LoadRecentTokenTransactions: {
      state = {
        ...state,
        loaded: false
      }
      break;
    }
    
    case TokensActionTypes.TokensLoaded: {
      state = {
        ...state,
        list  : action.payload,
        loaded: true
      };
      break;
    }

    case TokensActionTypes.TokensLoadError: {
      state = {
        ...state,
        loaded: true,
        error: action.payload
      };
      break;
    }
  }
  
  return state;
}
