import { TestBed, async } from '@angular/core/testing';

import { Observable } from 'rxjs';

import { EffectsModule } from '@ngrx/effects';
import { StoreModule } from '@ngrx/store';
import { provideMockActions } from '@ngrx/effects/testing';

import { NxModule } from '@nrwl/nx';
import { DataPersistence } from '@nrwl/nx';
import { hot } from '@nrwl/nx/testing';

import { SmartContractsEffects } from './smart-contracts.effects';
import {
  LoadSmartContracts,
  SmartContractsLoaded
} from './smart-contracts.actions';

describe('SmartContractsEffects', () => {
  let actions: Observable<any>;
  let effects: SmartContractsEffects;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [
        NxModule.forRoot(),
        StoreModule.forRoot({}),
        EffectsModule.forRoot([])
      ],
      providers: [
        SmartContractsEffects,
        DataPersistence,
        provideMockActions(() => actions)
      ]
    });

    effects = TestBed.get(SmartContractsEffects);
  });

  describe('loadSmartContracts$', () => {
    it('should work', () => {
      actions = hot('-a-|', { a: new LoadSmartContracts() });
      expect(effects.loadSmartContracts$).toBeObservable(
        hot('-a-|', { a: new SmartContractsLoaded([]) })
      );
    });
  });
});
