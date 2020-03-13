import { Injectable } from '@angular/core';
import { Effect, Actions } from '@ngrx/effects';
import { DataPersistence } from '@nrwl/nx';

import { TokensPartialState  } from './tokens.reducer';
import { LoadTokens, TokensLoaded, TokensLoadError, TokensActionTypes, LoadRecentTokenTransactions, LoadTokenDetail, TokenDetailLoaded, TokenDetailLoadError } from './tokens.actions';
import { TokensService } from '../services/tokens.service';
import { map } from 'rxjs/operators';

@Injectable()
export class TokensEffects {

 @Effect() loadTokens$ = this.dataPersistence.fetch(TokensActionTypes.LoadTokens, {
   run: (action: LoadTokens, state: TokensPartialState) => {
     // Your custom REST 'load' logic goes here. For now just return an empty list...
     let args = {
      tokenAddress: action.tokenAddress,
      filterAddress: action.filterAddress
     } as TokensService.TransactionsForTokenParams;

     return this.tokensService.TransactionsForToken(args).pipe(
       map(transactions => new TokensLoaded(transactions))
     );
   },

   onError: (action: LoadTokens, error) => {
     console.error('Error', error);
     return new TokensLoadError(error);
   }
 });


 @Effect() loadRecentTokens$ = this.dataPersistence.fetch(TokensActionTypes.LoadRecentTokenTransactions, {
  run: (action: LoadRecentTokenTransactions, state: TokensPartialState) => {
    return this.tokensService.RecentTransactionsForToken(action.tokenAddress).pipe(
      map(transactions => new TokensLoaded(transactions)
    ))
  },
  onError: (action: LoadRecentTokenTransactions, error) => {
    console.error('Error', error);
    return new TokensLoadError(error);
  }
 });

 @Effect() loadTokenDetail$ = this.dataPersistence.fetch(TokensActionTypes.LoadTokenDetail, {
  run: (action: LoadTokenDetail, state: TokensPartialState) => {
    return this.tokensService.TokenDetail(action.address).pipe(
      map(tokenDetail => new TokenDetailLoaded(tokenDetail)
    ))
  },
  onError: (action: LoadTokenDetail, error) => {
    console.error('Error', error);
    return new TokenDetailLoadError(error);
  }
 });

 constructor(
   private actions$: Actions,
   private tokensService: TokensService,
   private dataPersistence: DataPersistence<TokensPartialState>) { }
}
