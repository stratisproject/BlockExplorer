import { TokensState, TOKENS_FEATURE_KEY } from "./tokens.reducer";
import { createFeatureSelector } from "@ngrx/store";

const getTokensState = createFeatureSelector<TokensState>(
    TOKENS_FEATURE_KEY
);

export const transactionsQuery = {
// Put all the queries here
};
  