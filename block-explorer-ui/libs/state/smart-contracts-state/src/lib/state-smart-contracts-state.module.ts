import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StoreModule } from '@ngrx/store';
import { EffectsModule } from '@ngrx/effects';
import {
  SMARTCONTRACTS_FEATURE_KEY,
  initialState as smartContractsInitialState,
  smartContractsReducer
} from './+state/smart-contracts.reducer';
import { SmartContractsEffects } from './+state/smart-contracts.effects';
import { SmartContractsFacade } from './+state/smart-contracts.facade';

@NgModule({
  imports: [
    CommonModule,
    StoreModule.forFeature(SMARTCONTRACTS_FEATURE_KEY, smartContractsReducer, {
      initialState: smartContractsInitialState
    }),
    EffectsModule.forFeature([SmartContractsEffects])
  ],
  providers: [SmartContractsFacade]
})
export class StateSmartContractsStateModule {}
