import {Action} from '@ngrx/store';
import { TokenTransactionResponse } from '../services/token-transaction-response';

export enum TokensActionTypes {
 LoadTokens = "[Tokens] Load Tokens",
 TokensLoaded = "[Tokens] Tokens Loaded",
 TokensLoadError = "[Tokens] Tokens Load Error",
 LoadRecentTokenTransactions = "[Tokens] Load recent token transactions"
}

export class LoadTokens implements Action {
 readonly type = TokensActionTypes.LoadTokens;
}

export class TokensLoadError implements Action {
 readonly type = TokensActionTypes.TokensLoadError;
 constructor(public payload: any) { }
}

export class TokensLoaded implements Action {
 readonly type = TokensActionTypes.TokensLoaded;
 constructor(public payload: TokenTransactionResponse[]) { }
}

export class LoadRecentTokenTransactions implements Action {
  readonly type = TokensActionTypes.LoadRecentTokenTransactions;

  constructor(public tokenAddress: string) {}
}


export type TokensAction = LoadTokens | TokensLoaded | TokensLoadError | LoadRecentTokenTransactions;

export const fromTokensActions = {
  LoadTokens,
  TokensLoaded,
  TokensLoadError,
  LoadRecentTokenTransactions
};
