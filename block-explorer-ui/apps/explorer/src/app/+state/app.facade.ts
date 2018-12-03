import { Injectable } from '@angular/core';

import { select, Store } from '@ngrx/store';

import { AppPartialState } from './app.reducer';
import { appQuery } from './app.selectors';
import { LoadApp } from './app.actions';

@Injectable()
export class AppFacade {
  loaded$ = this.store.pipe(select(appQuery.getLoaded));
  allApp$ = this.store.pipe(select(appQuery.getAllApp));
  selectedApp$ = this.store.pipe(select(appQuery.getSelectedApp));

  constructor(private store: Store<AppPartialState>) {}

  loadAll() {
    this.store.dispatch(new LoadApp());
  }
}
