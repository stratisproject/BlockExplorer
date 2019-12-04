import { Injectable } from '@angular/core';
import { select, Store } from '@ngrx/store';
import * as fromActions from './actions';
import * as fromSelectors from './selectors';
import { SelectedBlockState } from './reducers/selected-block.reducer';

@Injectable({ providedIn: 'root' })
export class SelectedBlockFacade {
    loaded$ = this.store.pipe(select(fromSelectors.getIsSelectedBlockLoaded));
    error$ = this.store.pipe(select(fromSelectors.getSelectedBlockError$));
    block$ = this.store.pipe(select(fromSelectors.getSelectedBlock));

    constructor(private store: Store<SelectedBlockState>) { }

    loadBlock(blockHash: string) {
        this.store.dispatch(fromActions.loadBlock(blockHash));
    }
}
