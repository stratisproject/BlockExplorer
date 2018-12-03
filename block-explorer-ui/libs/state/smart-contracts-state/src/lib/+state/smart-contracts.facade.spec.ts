import { NgModule } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { readFirst } from '@nrwl/nx/testing';

import { EffectsModule } from '@ngrx/effects';
import { StoreModule, Store } from '@ngrx/store';

import { NxModule } from '@nrwl/nx';

import { SmartContractsEffects } from './smart-contracts.effects';
import { SmartContractsFacade } from './smart-contracts.facade';

import { smartContractsQuery } from './smart-contracts.selectors';
import {
  LoadSmartContracts,
  SmartContractsLoaded
} from './smart-contracts.actions';
import {
  SmartContractsState,
  Entity,
  initialState,
  smartContractsReducer
} from './smart-contracts.reducer';

interface TestSchema {
  smartContracts: SmartContractsState;
}

describe('SmartContractsFacade', () => {
  let facade: SmartContractsFacade;
  let store: Store<TestSchema>;
  let createSmartContracts;

  beforeEach(() => {
    createSmartContracts = (id: string, name = ''): Entity => ({
      id,
      name: name || `name-${id}`
    });
  });

  describe('used in NgModule', () => {
    beforeEach(() => {
      @NgModule({
        imports: [
          StoreModule.forFeature('smartContracts', smartContractsReducer, {
            initialState
          }),
          EffectsModule.forFeature([SmartContractsEffects])
        ],
        providers: [SmartContractsFacade]
      })
      class CustomFeatureModule {}

      @NgModule({
        imports: [
          NxModule.forRoot(),
          StoreModule.forRoot({}),
          EffectsModule.forRoot([]),
          CustomFeatureModule
        ]
      })
      class RootModule {}
      TestBed.configureTestingModule({ imports: [RootModule] });

      store = TestBed.get(Store);
      facade = TestBed.get(SmartContractsFacade);
    });

    /**
     * The initially generated facade::loadAll() returns empty array
     */
    it('loadAll() should return empty list with loaded == true', async done => {
      try {
        let list = await readFirst(facade.allSmartContracts$);
        let isLoaded = await readFirst(facade.loaded$);

        expect(list.length).toBe(0);
        expect(isLoaded).toBe(false);

        facade.loadAll();

        list = await readFirst(facade.allSmartContracts$);
        isLoaded = await readFirst(facade.loaded$);

        expect(list.length).toBe(0);
        expect(isLoaded).toBe(true);

        done();
      } catch (err) {
        done.fail(err);
      }
    });

    /**
     * Use `SmartContractsLoaded` to manually submit list for state management
     */
    it('allSmartContracts$ should return the loaded list; and loaded flag == true', async done => {
      try {
        let list = await readFirst(facade.allSmartContracts$);
        let isLoaded = await readFirst(facade.loaded$);

        expect(list.length).toBe(0);
        expect(isLoaded).toBe(false);

        store.dispatch(
          new SmartContractsLoaded([
            createSmartContracts('AAA'),
            createSmartContracts('BBB')
          ])
        );

        list = await readFirst(facade.allSmartContracts$);
        isLoaded = await readFirst(facade.loaded$);

        expect(list.length).toBe(2);
        expect(isLoaded).toBe(true);

        done();
      } catch (err) {
        done.fail(err);
      }
    });
  });
});
