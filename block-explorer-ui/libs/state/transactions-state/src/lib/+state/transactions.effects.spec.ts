import { TestBed, async } from '@angular/core/testing';

import { Observable } from 'rxjs';

import { EffectsModule } from '@ngrx/effects';
import { StoreModule } from '@ngrx/store';
import { provideMockActions } from '@ngrx/effects/testing';

import { NxModule } from '@nrwl/nx';
import { DataPersistence } from '@nrwl/nx';
import { hot } from '@nrwl/nx/testing';

import { TransactionsEffects } from './transactions.effects';
import { LoadTransactions, TransactionsLoaded } from './transactions.actions';

describe('TransactionsEffects', () => {
  let actions: Observable<any>;
  let effects: TransactionsEffects;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [
        NxModule.forRoot(),
        StoreModule.forRoot({}),
        EffectsModule.forRoot([])
      ],
      providers: [
        TransactionsEffects,
        DataPersistence,
        provideMockActions(() => actions)
      ]
    });

    effects = TestBed.get(TransactionsEffects);
  });

  describe('loadTransactions$', () => {
    it('should work', () => {
      actions = hot('-a-|', { a: new LoadTransactions() });
      expect(effects.loadTransactions$).toBeObservable(
        hot('-a-|', { a: new TransactionsLoaded([]) })
      );
    });
  });
});
