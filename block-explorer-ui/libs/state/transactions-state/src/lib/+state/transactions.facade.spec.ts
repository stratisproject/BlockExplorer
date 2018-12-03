import { NgModule } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { readFirst } from '@nrwl/nx/testing';

import { EffectsModule } from '@ngrx/effects';
import { StoreModule, Store } from '@ngrx/store';

import { NxModule } from '@nrwl/nx';

import { TransactionsEffects } from './transactions.effects';
import { TransactionsFacade } from './transactions.facade';

import { transactionsQuery } from './transactions.selectors';
import { LoadTransactions, TransactionsLoaded } from './transactions.actions';
import {
  TransactionsState,
  Entity,
  initialState,
  transactionsReducer
} from './transactions.reducer';

interface TestSchema {
  transactions: TransactionsState;
}

describe('TransactionsFacade', () => {
  let facade: TransactionsFacade;
  let store: Store<TestSchema>;
  let createTransactions;

  beforeEach(() => {
    createTransactions = (id: string, name = ''): Entity => ({
      id,
      name: name || `name-${id}`
    });
  });

  describe('used in NgModule', () => {
    beforeEach(() => {
      @NgModule({
        imports: [
          StoreModule.forFeature('transactions', transactionsReducer, {
            initialState
          }),
          EffectsModule.forFeature([TransactionsEffects])
        ],
        providers: [TransactionsFacade]
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
      facade = TestBed.get(TransactionsFacade);
    });

    /**
     * The initially generated facade::loadAll() returns empty array
     */
    it('loadAll() should return empty list with loaded == true', async done => {
      try {
        let list = await readFirst(facade.allTransactions$);
        let isLoaded = await readFirst(facade.loaded$);

        expect(list.length).toBe(0);
        expect(isLoaded).toBe(false);

        facade.loadAll();

        list = await readFirst(facade.allTransactions$);
        isLoaded = await readFirst(facade.loaded$);

        expect(list.length).toBe(0);
        expect(isLoaded).toBe(true);

        done();
      } catch (err) {
        done.fail(err);
      }
    });

    /**
     * Use `TransactionsLoaded` to manually submit list for state management
     */
    it('allTransactions$ should return the loaded list; and loaded flag == true', async done => {
      try {
        let list = await readFirst(facade.allTransactions$);
        let isLoaded = await readFirst(facade.loaded$);

        expect(list.length).toBe(0);
        expect(isLoaded).toBe(false);

        store.dispatch(
          new TransactionsLoaded([
            createTransactions('AAA'),
            createTransactions('BBB')
          ])
        );

        list = await readFirst(facade.allTransactions$);
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
