import { Injectable } from '@angular/core';

import { select, Store } from '@ngrx/store';

import { TokensPartialState } from './tokens.reducer';
import { tokensQuery } from './tokens.selectors';
import { LoadTokens, LoadRecentTokenTransactions, LoadTokenDetail } from './tokens.actions';

@Injectable()
export class TokensFacade {

  loaded$ = this.store.pipe(select(tokensQuery.getLoaded));
  allTokens$ = this.store.pipe(select(tokensQuery.getAllTokens));
  selectedTokens$ = this.store.pipe(select(tokensQuery.getSelectedTokens));
  detailLoaded$ = this.store.pipe(select(tokensQuery.getSelectedDetailLoading));
  selectedDetail$ = this.store.pipe(select(tokensQuery.getSelectedDetail));
  
  constructor(private store: Store<TokensPartialState>) { }
 
  loadAll(tokenAddress: string, filterAddress?: string) {
    this.store.dispatch(new LoadTokens(tokenAddress, filterAddress));
  }

  loadRecent(tokenAddress: string) {
    this.store.dispatch(new LoadRecentTokenTransactions(tokenAddress));
  }

  loadDetail(tokenAddress: string) {
    this.store.dispatch(new LoadTokenDetail(tokenAddress));
  }
}
