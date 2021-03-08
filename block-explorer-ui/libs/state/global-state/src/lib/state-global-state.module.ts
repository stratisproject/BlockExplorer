import { NgModule, ModuleWithProviders } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StoreModule } from '@ngrx/store';
import { EffectsModule } from '@ngrx/effects';
import {
  GLOBAL_FEATURE_KEY,
  initialState as globalInitialState,
  globalReducer
} from './+state/global.reducer';
import { GlobalEffects } from './+state/global.effects';
import { GlobalFacade } from './+state/global.facade';
import { FinderService } from './services/finder.service';

@NgModule({
  imports: [
    CommonModule,
    StoreModule.forFeature(GLOBAL_FEATURE_KEY, globalReducer, {
      initialState: globalInitialState
    }),
    EffectsModule.forFeature([GlobalEffects])
  ],
  providers: [GlobalFacade]
})
export class StateGlobalStateModule  {
  static forRoot(): ModuleWithProviders {
      return {
          ngModule: StateGlobalStateModule,
          providers: [FinderService]
      };
  }
}
