import { Injectable } from '@angular/core';
import { select, Store } from '@ngrx/store';
import * as fromActions from './actions';
import * as fromSelectors from './selectors';
import { LastBlocksState } from './reducers/last-blocks.reducer';

@Injectable({ providedIn: 'root' })
export class LastBlocksFacade {
    loaded$ = this.store.pipe(select(fromSelectors.getAreLastBlocksLoaded));
    blocks$ = this.store.pipe(select(fromSelectors.getLastBlocks));
    error$ = this.store.pipe(select(fromSelectors.loadLastBlocksError$));

    constructor(private store: Store<LastBlocksState>) { }

    loadLastBlocks(records: number) {
        this.store.dispatch(fromActions.loadLastBlocks(records));
    }
}
