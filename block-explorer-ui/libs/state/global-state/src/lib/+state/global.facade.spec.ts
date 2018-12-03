import { NgModule } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { readFirst } from '@nrwl/nx/testing';

import { EffectsModule } from '@ngrx/effects';
import { StoreModule, Store } from '@ngrx/store';

import { NxModule } from '@nrwl/nx';

import { GlobalEffects } from './global.effects';
import { GlobalFacade } from './global.facade';

import { globalQuery } from './global.selectors';
import { LoadGlobal, GlobalLoaded } from './global.actions';
import {
  GlobalState,
  Entity,
  initialState,
  globalReducer
} from './global.reducer';

interface TestSchema {
  global: GlobalState;
}

describe('GlobalFacade', () => {
  let facade: GlobalFacade;
  let store: Store<TestSchema>;
  let createGlobal;

  beforeEach(() => {
    createGlobal = (id: string, name = ''): Entity => ({
      id,
      name: name || `name-${id}`
    });
  });

  describe('used in NgModule', () => {
    beforeEach(() => {
      @NgModule({
        imports: [
          StoreModule.forFeature('global', globalReducer, { initialState }),
          EffectsModule.forFeature([GlobalEffects])
        ],
        providers: [GlobalFacade]
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
      facade = TestBed.get(GlobalFacade);
    });

    /**
     * The initially generated facade::loadAll() returns empty array
     */
    it('loadAll() should return empty list with loaded == true', async done => {
      try {
        let list = await readFirst(facade.allGlobal$);
        let isLoaded = await readFirst(facade.loaded$);

        expect(list.length).toBe(0);
        expect(isLoaded).toBe(false);

        facade.loadAll();

        list = await readFirst(facade.allGlobal$);
        isLoaded = await readFirst(facade.loaded$);

        expect(list.length).toBe(0);
        expect(isLoaded).toBe(true);

        done();
      } catch (err) {
        done.fail(err);
      }
    });

    /**
     * Use `GlobalLoaded` to manually submit list for state management
     */
    it('allGlobal$ should return the loaded list; and loaded flag == true', async done => {
      try {
        let list = await readFirst(facade.allGlobal$);
        let isLoaded = await readFirst(facade.loaded$);

        expect(list.length).toBe(0);
        expect(isLoaded).toBe(false);

        store.dispatch(
          new GlobalLoaded([createGlobal('AAA'), createGlobal('BBB')])
        );

        list = await readFirst(facade.allGlobal$);
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
