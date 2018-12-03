import { Entity, GlobalState } from './global.reducer';
import { globalQuery } from './global.selectors';

describe('Global Selectors', () => {
  const ERROR_MSG = 'No Error Available';
  const getGlobalId = it => it['id'];

  let storeState;

  beforeEach(() => {
    const createGlobal = (id: string, name = ''): Entity => ({
      id,
      name: name || `name-${id}`
    });
    storeState = {
      global: {
        list: [
          createGlobal('PRODUCT-AAA'),
          createGlobal('PRODUCT-BBB'),
          createGlobal('PRODUCT-CCC')
        ],
        selectedId: 'PRODUCT-BBB',
        error: ERROR_MSG,
        loaded: true
      }
    };
  });

  describe('Global Selectors', () => {
    it('getAllGlobal() should return the list of Global', () => {
      const results = globalQuery.getAllGlobal(storeState);
      const selId = getGlobalId(results[1]);

      expect(results.length).toBe(3);
      expect(selId).toBe('PRODUCT-BBB');
    });

    it('getSelectedGlobal() should return the selected Entity', () => {
      const result = globalQuery.getSelectedGlobal(storeState);
      const selId = getGlobalId(result);

      expect(selId).toBe('PRODUCT-BBB');
    });

    it("getLoaded() should return the current 'loaded' status", () => {
      const result = globalQuery.getLoaded(storeState);

      expect(result).toBe(true);
    });

    it("getError() should return the current 'error' storeState", () => {
      const result = globalQuery.getError(storeState);

      expect(result).toBe(ERROR_MSG);
    });
  });
});
