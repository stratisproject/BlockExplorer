import { TestBed, async } from '@angular/core/testing';

import { Observable } from 'rxjs';

import { EffectsModule } from '@ngrx/effects';
import { StoreModule } from '@ngrx/store';
import { provideMockActions } from '@ngrx/effects/testing';

import { NxModule } from '@nrwl/nx';
import { DataPersistence } from '@nrwl/nx';
import { hot } from '@nrwl/nx/testing';

import { TokensEffects } from './tokens.effects';
import { LoadTokens, TokensLoaded } from './tokens.actions';

describe('TokensEffects', () => {
  let actions: Observable<any>;
  let effects: TokensEffects;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [
        NxModule.forRoot(),
        StoreModule.forRoot({}),
        EffectsModule.forRoot([])
      ],
      providers: [
        TokensEffects,
        DataPersistence,
        provideMockActions(() => actions)
      ]
    });

    effects = TestBed.get(TokensEffects);
  });

  describe('loadTokens$', () => {
    it('should work', () => {
      actions = hot('-a-|', { a: new LoadTokens() });
      expect(effects.loadTokens$).toBeObservable(
        hot('-a-|', { a: new TokensLoaded([]) })
      );
    });
  });
});
