import { Injectable } from '@angular/core';
import { select, Store } from '@ngrx/store';
import * as fromActions from './actions';
import * as fromSelectors from './selectors';
import { BlocksState } from './reducers/blocks.reducer';

@Injectable({ providedIn: 'root' })
export class BlocksFacade {
    blocks$ = this.store.pipe(select(fromSelectors.getBlocks));
    loaded$ = this.store.pipe(select(fromSelectors.getAreBlocksLoaded));
    error$ = this.store.pipe(select(fromSelectors.getSelectedBlockError$));

    constructor(private store: Store<BlocksState>) { }

    loadBlocks(records: number) {
        this.store.dispatch(fromActions.loadBlocks(records));
    }
}
