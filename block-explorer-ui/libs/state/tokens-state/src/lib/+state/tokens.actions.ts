import { Action } from "@ngrx/store";

export enum TokensActionTypes {
    LoadTokens = '[Tokens] Load tokens'
}

export class LoadTokens implements Action {
    readonly type = TokensActionTypes.LoadTokens;
}

export type TokensActions =
LoadTokens;
