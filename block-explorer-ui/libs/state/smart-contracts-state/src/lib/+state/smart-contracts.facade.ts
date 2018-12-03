import { Injectable } from '@angular/core';

import { select, Store } from '@ngrx/store';

import { SmartContractsPartialState } from './smart-contracts.reducer';
import { smartContractsQuery } from './smart-contracts.selectors';
import { LoadSmartContracts } from './smart-contracts.actions';

@Injectable()
export class SmartContractsFacade {
  loaded$ = this.store.pipe(select(smartContractsQuery.getLoaded));
  allSmartContracts$ = this.store.pipe(
    select(smartContractsQuery.getAllSmartContracts)
  );
  selectedSmartContracts$ = this.store.pipe(
    select(smartContractsQuery.getSelectedSmartContracts)
  );

  constructor(private store: Store<SmartContractsPartialState>) {}

  loadAll() {
    this.store.dispatch(new LoadSmartContracts());
  }
}
