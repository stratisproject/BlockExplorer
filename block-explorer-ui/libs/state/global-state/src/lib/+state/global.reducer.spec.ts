import { GlobalLoaded } from './global.actions';
import {
  GlobalState,
  Entity,
  initialState,
  globalReducer
} from './global.reducer';

describe('Global Reducer', () => {
  const getGlobalId = it => it['id'];
  let createGlobal;

  beforeEach(() => {
    createGlobal = (id: string, name = ''): Entity => ({
      id,
      name: name || `name-${id}`
    });
  });

  describe('valid Global actions ', () => {
    it('should return set the list of known Global', () => {
      const globals = [
        createGlobal('PRODUCT-AAA'),
        createGlobal('PRODUCT-zzz')
      ];
      const action = new GlobalLoaded(globals);
      const result: GlobalState = globalReducer(initialState, action);
      const selId: string = getGlobalId(result.list[1]);

      expect(result.loaded).toBe(true);
      expect(result.list.length).toBe(2);
      expect(selId).toBe('PRODUCT-zzz');
    });
  });

  describe('unknown action', () => {
    it('should return the initial state', () => {
      const action = {} as any;
      const result = globalReducer(initialState, action);

      expect(result).toBe(initialState);
    });
  });
});
