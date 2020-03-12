import { Injectable } from '@angular/core';
import { select, Store } from '@ngrx/store';
import { TokensPartialState } from './tokens.reducer';
import { LoadRecentTokenTransactions } from './tokens.actions';

@Injectable()
export class TokensFacade {

  constructor(private store: Store<TokensPartialState>) {}

  loadRecent(tokenAddress: string) {
    this.store.dispatch(new LoadRecentTokenTransactions(tokenAddress));
  }
}