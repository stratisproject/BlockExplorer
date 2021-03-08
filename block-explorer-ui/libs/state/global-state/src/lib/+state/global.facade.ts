import { Injectable } from '@angular/core';

import { select, Store } from '@ngrx/store';

import { GlobalPartialState } from './global.reducer';
import { globalQuery } from './global.selectors';
import { IndentifyEntity } from './global.actions';

@Injectable()
export class GlobalFacade {
  identifiedEntity$ = this.store.pipe(select(globalQuery.getIdentifiedEntity));
  identifiedEntityType$ = this.store.pipe(select(globalQuery.getIdentifiedType));
  loaded$ = this.store.pipe(select(globalQuery.getLoaded));
  error$ = this.store.pipe(select(globalQuery.getError));

  constructor(private store: Store<GlobalPartialState>) {}

  identify(id: string) {
    this.store.dispatch(new IndentifyEntity(id));
  }
}
