import { Injectable } from '@angular/core';

import { select, Store } from '@ngrx/store';

import { GlobalPartialState } from './global.reducer';
import { globalQuery } from './global.selectors';
import { LoadGlobal } from './global.actions';

@Injectable()
export class GlobalFacade {
  loaded$ = this.store.pipe(select(globalQuery.getLoaded));
  allGlobal$ = this.store.pipe(select(globalQuery.getAllGlobal));
  selectedGlobal$ = this.store.pipe(select(globalQuery.getSelectedGlobal));

  constructor(private store: Store<GlobalPartialState>) {}

  loadAll() {
    this.store.dispatch(new LoadGlobal());
  }
}
