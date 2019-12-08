import { Injectable } from '@angular/core';
import { select, Store } from '@ngrx/store';
import * as fromActions from './actions';
import * as fromSelectors from './selectors';
import { BlocksState } from './reducers/blocks.reducer';

@Injectable({ providedIn: 'root' })
export class BlocksFacade {
    blocksLoaded$ = this.blocksStore.pipe(select(fromSelectors.getBlocksLoaded$));
    blocksError$ = this.blocksStore.pipe(select(fromSelectors.getBlocksError$));
    blocks$ = this.blocksStore.pipe(select(fromSelectors.getBlocks$));

    blockLoaded$ = this.blocksStore.pipe(select(fromSelectors.getBlockLoaded$));
    blockError$ = this.blocksStore.pipe(select(fromSelectors.getBlockError$));
    block$ = this.blocksStore.pipe(select(fromSelectors.getBlock$));

    constructor(private blocksStore: Store<BlocksState>) { }

    getBlocks(from: number = 0, records: number = 10) {
        this.blocksStore.dispatch(fromActions.loadBlocks(from, records));
    }

    getBlock(id: string | number) {
        this.blocksStore.dispatch(fromActions.loadBlock(id));
    }
}
