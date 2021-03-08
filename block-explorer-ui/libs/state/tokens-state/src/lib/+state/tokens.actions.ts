import {Action} from '@ngrx/store';
import { TokenTransactionResponse } from '../services/token-transaction-response';
import { TokenDetail } from '../services/token-detail';

export enum TokensActionTypes {
 LoadTokens = "[Tokens] Load Tokens",
 TokensLoaded = "[Tokens] Tokens Loaded",
 TokensLoadError = "[Tokens] Tokens Load Error",
 LoadRecentTokenTransactions = "[Tokens] Load recent token transactions",

 LoadTokenDetail = "[Tokens] Load token detail",
 TokenDetailLoaded = "[Tokens] Token detail loaded",
 TokenDetailLoadError = "[Tokens] Token detail load error",
}

export class LoadTokenDetail implements Action {
  readonly type = TokensActionTypes.LoadTokenDetail;

  constructor(public address: string) {}
}

export class TokenDetailLoaded implements Action {
  readonly type = TokensActionTypes.TokenDetailLoaded;
  constructor(public payload: TokenDetail) { }
}

export class TokenDetailLoadError implements Action {
  readonly type = TokensActionTypes.TokenDetailLoadError;
  constructor(public payload: any) { }
}

export class LoadTokens implements Action {
 readonly type = TokensActionTypes.LoadTokens;
 constructor(public tokenAddress: string, public filterAddress?: string) {}
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


export type TokensAction = LoadTokens | TokensLoaded | TokensLoadError | LoadRecentTokenTransactions
| LoadTokenDetail | TokenDetailLoaded | TokenDetailLoadError;

export const fromTokensActions = {
  LoadTokens,
  TokensLoaded,
  TokensLoadError,
  LoadRecentTokenTransactions,

  LoadTokenDetail,
  TokenDetailLoaded,
  TokenDetailLoadError
};
