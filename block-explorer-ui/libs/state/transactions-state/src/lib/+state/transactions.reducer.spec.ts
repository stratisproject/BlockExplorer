import { TransactionsLoaded } from './transactions.actions';
import {
  TransactionsState,
  Entity,
  initialState,
  transactionsReducer
} from './transactions.reducer';

describe('Transactions Reducer', () => {
  const getTransactionsId = it => it['id'];
  let createTransactions;

  beforeEach(() => {
    createTransactions = (id: string, name = ''): Entity => ({
      id,
      name: name || `name-${id}`
    });
  });

  describe('valid Transactions actions ', () => {
    it('should return set the list of known Transactions', () => {
      const transactionss = [
        createTransactions('PRODUCT-AAA'),
        createTransactions('PRODUCT-zzz')
      ];
      const action = new TransactionsLoaded(transactionss);
      const result: TransactionsState = transactionsReducer(
        initialState,
        action
      );
      const selId: string = getTransactionsId(result.list[1]);

      expect(result.loaded).toBe(true);
      expect(result.list.length).toBe(2);
      expect(selId).toBe('PRODUCT-zzz');
    });
  });

  describe('unknown action', () => {
    it('should return the initial state', () => {
      const action = {} as any;
      const result = transactionsReducer(initialState, action);

      expect(result).toBe(initialState);
    });
  });
});
