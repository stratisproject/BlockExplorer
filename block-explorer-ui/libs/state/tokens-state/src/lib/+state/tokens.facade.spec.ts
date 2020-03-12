import { NgModule } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { readFirst } from '@nrwl/nx/testing';

import { EffectsModule } from '@ngrx/effects';
import { StoreModule, Store } from '@ngrx/store';

import { NxModule } from '@nrwl/nx';

import { TokensEffects } from './tokens.effects';
import { TokensFacade } from './tokens.facade';

import { tokensQuery } from './tokens.selectors';
import { LoadTokens, TokensLoaded } from './tokens.actions';
import {
  TokensState,
  Entity,
  initialState,
  tokensReducer
} from './tokens.reducer';

interface TestSchema {
  tokens: TokensState;
}

describe('TokensFacade', () => {
  let facade: TokensFacade;
  let store: Store<TestSchema>;
  let createTokens;

  beforeEach(() => {
    createTokens = (id: string, name = ''): Entity => ({
      id,
      name: name || `name-${id}`
    });
  });

  describe('used in NgModule', () => {
    beforeEach(() => {
      @NgModule({
        imports: [
          StoreModule.forFeature('tokens', tokensReducer, { initialState }),
          EffectsModule.forFeature([TokensEffects])
        ],
        providers: [TokensFacade]
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
      facade = TestBed.get(TokensFacade);
    });

    /**
     * The initially generated facade::loadAll() returns empty array
     */
    it('loadAll() should return empty list with loaded == true', async done => {
      try {
        let list = await readFirst(facade.allTokens$);
        let isLoaded = await readFirst(facade.loaded$);

        expect(list.length).toBe(0);
        expect(isLoaded).toBe(false);

        facade.loadAll();

        list = await readFirst(facade.allTokens$);
        isLoaded = await readFirst(facade.loaded$);

        expect(list.length).toBe(0);
        expect(isLoaded).toBe(true);

        done();
      } catch (err) {
        done.fail(err);
      }
    });

    /**
     * Use `TokensLoaded` to manually submit list for state management
     */
    it('allTokens$ should return the loaded list; and loaded flag == true', async done => {
      try {
        let list = await readFirst(facade.allTokens$);
        let isLoaded = await readFirst(facade.loaded$);

        expect(list.length).toBe(0);
        expect(isLoaded).toBe(false);

        store.dispatch(
          new TokensLoaded([createTokens('AAA'), createTokens('BBB')])
        );

        list = await readFirst(facade.allTokens$);
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
