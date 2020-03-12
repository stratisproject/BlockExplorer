import { createFeatureSelector, createSelector } from '@ngrx/store';
import { TOKENS_FEATURE_KEY, TokensState } from './tokens.reducer';

// Lookup the 'Tokens' feature state managed by NgRx
const getTokensState = createFeatureSelector<TokensState>(TOKENS_FEATURE_KEY);

const getLoaded = createSelector( getTokensState, (state:TokensState) => state.loaded );
const getError = createSelector( getTokensState, (state:TokensState) => state.error );

const getAllTokens = createSelector( getTokensState, getLoaded, (state:TokensState, isLoaded) => {
  return isLoaded ? state.list : [ ];
});
const getSelectedId = createSelector( getTokensState, (state:TokensState) => state.selectedId );
const getSelectedTokens = createSelector( getAllTokens, getSelectedId, (tokens, id) => {
    const result = tokens.find(it => it['id'] === id);
    return result ? Object.assign({}, result) : undefined;
});

export const tokensQuery = {
  getLoaded,
  getError,
  getAllTokens,
  getSelectedTokens
};
