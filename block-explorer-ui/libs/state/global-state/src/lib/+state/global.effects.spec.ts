import { TestBed, async } from '@angular/core/testing';

import { Observable } from 'rxjs';

import { EffectsModule } from '@ngrx/effects';
import { StoreModule } from '@ngrx/store';
import { provideMockActions } from '@ngrx/effects/testing';

import { NxModule } from '@nrwl/nx';
import { DataPersistence } from '@nrwl/nx';
import { hot } from '@nrwl/nx/testing';

import { GlobalEffects } from './global.effects';
import { LoadGlobal, GlobalLoaded } from './global.actions';

describe('GlobalEffects', () => {
  let actions: Observable<any>;
  let effects: GlobalEffects;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [
        NxModule.forRoot(),
        StoreModule.forRoot({}),
        EffectsModule.forRoot([])
      ],
      providers: [
        GlobalEffects,
        DataPersistence,
        provideMockActions(() => actions)
      ]
    });

    effects = TestBed.get(GlobalEffects);
  });

  describe('loadGlobal$', () => {
    it('should work', () => {
      actions = hot('-a-|', { a: new LoadGlobal() });
      expect(effects.loadGlobal$).toBeObservable(
        hot('-a-|', { a: new GlobalLoaded([]) })
      );
    });
  });
});
