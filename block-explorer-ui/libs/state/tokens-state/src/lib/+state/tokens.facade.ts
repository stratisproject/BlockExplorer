import { Injectable } from '@angular/core';
import { select, Store } from '@ngrx/store';
import { TokensPartialState } from './tokens.reducer';

@Injectable()
export class TokensFacade {

  constructor(private store: Store<TokensPartialState>) {}
}