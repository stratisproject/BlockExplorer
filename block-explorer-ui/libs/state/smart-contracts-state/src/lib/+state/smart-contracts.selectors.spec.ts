import { Entity, SmartContractsState } from './smart-contracts.reducer';
import { smartContractsQuery } from './smart-contracts.selectors';

describe('SmartContracts Selectors', () => {
  const ERROR_MSG = 'No Error Available';
  const getSmartContractsId = it => it['id'];

  let storeState;

  beforeEach(() => {
    const createSmartContracts = (id: string, name = ''): Entity => ({
      id,
      name: name || `name-${id}`
    });
    storeState = {
      smartContracts: {
        list: [
          createSmartContracts('PRODUCT-AAA'),
          createSmartContracts('PRODUCT-BBB'),
          createSmartContracts('PRODUCT-CCC')
        ],
        selectedId: 'PRODUCT-BBB',
        error: ERROR_MSG,
        loaded: true
      }
    };
  });

  describe('SmartContracts Selectors', () => {
    it('getAllSmartContracts() should return the list of SmartContracts', () => {
      const results = smartContractsQuery.getAllSmartContracts(storeState);
      const selId = getSmartContractsId(results[1]);

      expect(results.length).toBe(3);
      expect(selId).toBe('PRODUCT-BBB');
    });

    it('getSelectedSmartContracts() should return the selected Entity', () => {
      const result = smartContractsQuery.getSelectedSmartContracts(storeState);
      const selId = getSmartContractsId(result);

      expect(selId).toBe('PRODUCT-BBB');
    });

    it("getLoaded() should return the current 'loaded' status", () => {
      const result = smartContractsQuery.getLoaded(storeState);

      expect(result).toBe(true);
    });

    it("getError() should return the current 'error' storeState", () => {
      const result = smartContractsQuery.getError(storeState);

      expect(result).toBe(ERROR_MSG);
    });
  });
});
