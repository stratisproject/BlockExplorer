import { NgModule, ModuleWithProviders } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StoreModule } from '@ngrx/store';
import { EffectsModule } from '@ngrx/effects';
import { TOKENS_FEATURE_KEY, initialState as tokensInitialState, tokensReducer } from './+state/tokens.reducer';
import { TokensEffects } from './+state/tokens.effects';
import { TokensFacade } from './+state/tokens.facade';
        
@NgModule({
  imports: [
    CommonModule,
    StoreModule.forFeature(TOKENS_FEATURE_KEY, tokensReducer, { initialState: tokensInitialState }),
    EffectsModule.forFeature([TokensEffects])
  ],
  providers: [TokensFacade]
})
export class StateTokensStateModule { 
    static forRoot(): ModuleWithProviders {
      return {
          ngModule: StateTokensStateModule,
          providers: []
      };
  }
}
