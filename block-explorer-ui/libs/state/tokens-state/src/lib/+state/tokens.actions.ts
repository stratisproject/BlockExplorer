import { Action } from "@ngrx/store";

export enum TokensActionTypes {
    LoadTokens = '[Tokens] Load tokens',
    LoadRecentTokenTransactions = '[Tokens] Load recent token transactions'
}

export class LoadTokens implements Action {
    readonly type = TokensActionTypes.LoadTokens;
}

export class LoadRecentTokenTransactions implements Action {
    readonly type = TokensActionTypes.LoadRecentTokenTransactions;

    constructor(public tokenAddress: string) {}
}

export type TokensActions =
LoadRecentTokenTransactions |
LoadTokens;
