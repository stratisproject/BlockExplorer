import { SmartContractsLoaded } from './smart-contracts.actions';
import {
  SmartContractsState,
  Entity,
  initialState,
  smartContractsReducer
} from './smart-contracts.reducer';

describe('SmartContracts Reducer', () => {
  const getSmartContractsId = it => it['id'];
  let createSmartContracts;

  beforeEach(() => {
    createSmartContracts = (id: string, name = ''): Entity => ({
      id,
      name: name || `name-${id}`
    });
  });

  describe('valid SmartContracts actions ', () => {
    it('should return set the list of known SmartContracts', () => {
      const smartContractss = [
        createSmartContracts('PRODUCT-AAA'),
        createSmartContracts('PRODUCT-zzz')
      ];
      const action = new SmartContractsLoaded(smartContractss);
      const result: SmartContractsState = smartContractsReducer(
        initialState,
        action
      );
      const selId: string = getSmartContractsId(result.list[1]);

      expect(result.loaded).toBe(true);
      expect(result.list.length).toBe(2);
      expect(selId).toBe('PRODUCT-zzz');
    });
  });

  describe('unknown action', () => {
    it('should return the initial state', () => {
      const action = {} as any;
      const result = smartContractsReducer(initialState, action);

      expect(result).toBe(initialState);
    });
  });
});
