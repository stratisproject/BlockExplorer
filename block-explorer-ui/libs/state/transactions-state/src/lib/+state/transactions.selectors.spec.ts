import { Entity, TransactionsState } from './transactions.reducer';
import { transactionsQuery } from './transactions.selectors';

describe('Transactions Selectors', () => {
  const ERROR_MSG = 'No Error Available';
  const getTransactionsId = it => it['id'];

  let storeState;

  beforeEach(() => {
    const createTransactions = (id: string, name = ''): Entity => ({
      id,
      name: name || `name-${id}`
    });
    storeState = {
      transactions: {
        list: [
          createTransactions('PRODUCT-AAA'),
          createTransactions('PRODUCT-BBB'),
          createTransactions('PRODUCT-CCC')
        ],
        selectedId: 'PRODUCT-BBB',
        error: ERROR_MSG,
        loaded: true
      }
    };
  });

  describe('Transactions Selectors', () => {
    it('getAllTransactions() should return the list of Transactions', () => {
      const results = transactionsQuery.getAllTransactions(storeState);
      const selId = getTransactionsId(results[1]);

      expect(results.length).toBe(3);
      expect(selId).toBe('PRODUCT-BBB');
    });

    it('getSelectedTransactions() should return the selected Entity', () => {
      const result = transactionsQuery.getSelectedTransactions(storeState);
      const selId = getTransactionsId(result);

      expect(selId).toBe('PRODUCT-BBB');
    });

    it("getLoaded() should return the current 'loaded' status", () => {
      const result = transactionsQuery.getLoaded(storeState);

      expect(result).toBe(true);
    });

    it("getError() should return the current 'error' storeState", () => {
      const result = transactionsQuery.getError(storeState);

      expect(result).toBe(ERROR_MSG);
    });
  });
});
