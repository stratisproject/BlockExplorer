import { Injectable } from '@angular/core';

import { select, Store } from '@ngrx/store';

import { TokensPartialState } from './tokens.reducer';
import { tokensQuery } from './tokens.selectors';
import { LoadTokens, LoadRecentTokenTransactions } from './tokens.actions';

@Injectable()
export class TokensFacade {

  loaded$ = this.store.pipe(select(tokensQuery.getLoaded));
  allTokens$ = this.store.pipe(select(tokensQuery.getAllTokens));
  selectedTokens$ = this.store.pipe(select(tokensQuery.getSelectedTokens));
  
  constructor(private store: Store<TokensPartialState>) { }
 
  loadAll() {
    this.store.dispatch(new LoadTokens());
  } 

  loadRecent(tokenAddress: string) {
    this.store.dispatch(new LoadRecentTokenTransactions(tokenAddress));
  }
}
