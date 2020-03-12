import { Entity, TokensState } from './tokens.reducer';
import { tokensQuery } from './tokens.selectors';

describe('Tokens Selectors', () => {
  const ERROR_MSG = 'No Error Available';
  const getTokensId = it => it['id'];

  let storeState;

  beforeEach(() => {
    const createTokens = (id: string, name = ''): Entity => ({
      id,
      name: name || `name-${id}`
    });
    storeState = {
      tokens: {
        list: [
          createTokens('PRODUCT-AAA'),
          createTokens('PRODUCT-BBB'),
          createTokens('PRODUCT-CCC')
        ],
        selectedId: 'PRODUCT-BBB',
        error: ERROR_MSG,
        loaded: true
      }
    };
  });

  describe('Tokens Selectors', () => {
    it('getAllTokens() should return the list of Tokens', () => {
      const results = tokensQuery.getAllTokens(storeState);
      const selId = getTokensId(results[1]);

      expect(results.length).toBe(3);
      expect(selId).toBe('PRODUCT-BBB');
    });

    it('getSelectedTokens() should return the selected Entity', () => {
      const result = tokensQuery.getSelectedTokens(storeState);
      const selId = getTokensId(result);

      expect(selId).toBe('PRODUCT-BBB');
    });

    it("getLoaded() should return the current 'loaded' status", () => {
      const result = tokensQuery.getLoaded(storeState);

      expect(result).toBe(true);
    });

    it("getError() should return the current 'error' storeState", () => {
      const result = tokensQuery.getError(storeState);

      expect(result).toBe(ERROR_MSG);
    });
  });
});
