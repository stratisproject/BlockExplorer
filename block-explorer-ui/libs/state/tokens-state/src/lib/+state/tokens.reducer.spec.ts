import { TokensLoaded } from './tokens.actions';
import {
  TokensState,
  Entity,
  initialState,
  tokensReducer
} from './tokens.reducer';

describe('Tokens Reducer', () => {
  const getTokensId = it => it['id'];
  let createTokens;

  beforeEach(() => {
    createTokens = (id: string, name = ''): Entity => ({
      id,
      name: name || `name-${id}`
    });
  });

  describe('valid Tokens actions ', () => {
    it('should return set the list of known Tokens', () => {
      const tokenss = [
        createTokens('PRODUCT-AAA'),
        createTokens('PRODUCT-zzz')
      ];
      const action = new TokensLoaded(tokenss);
      const result: TokensState = tokensReducer(initialState, action);
      const selId: string = getTokensId(result.list[1]);

      expect(result.loaded).toBe(true);
      expect(result.list.length).toBe(2);
      expect(selId).toBe('PRODUCT-zzz');
    });
  });

  describe('unknown action', () => {
    it('should return the initial state', () => {
      const action = {} as any;
      const result = tokensReducer(initialState, action);

      expect(result).toBe(initialState);
    });
  });
});
