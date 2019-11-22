import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import * as fromStore from '../../store';
import { Observable, interval, ReplaySubject, of } from 'rxjs';
import { Store, select } from '@ngrx/store';
import { BlockState } from '../../store';
import { startWith, takeUntil, switchMap } from 'rxjs/operators';
import { BlockSummaryModel } from '../../models/block-summary.model';

@Component({
    selector: 'app-latest-blocks',
    templateUrl: './latest-blocks.component.html',
    styleUrls: ['./latest-blocks.component.scss']
})
export class LatestBlocksComponent implements OnInit, OnDestroy {
    @Input() records: number = 0;
    @Input() autorefresh: number = 0;

    destroyed$ = new ReplaySubject<boolean>();
    displayedColumns = ['height', 'age', 'confirmations', 'transactionsCount', 'hash'];
    //dataSource;

    blocksLoaded$: Observable<boolean>;
    blocks$: Observable<BlockSummaryModel[]>;

    constructor(private store: Store<BlockState>) { }

    ngOnInit() {
        this.blocksLoaded$ = this.store.pipe(select(fromStore.getLastBlocksLoaded));
        this.blocks$ = this.store.pipe(
            select(fromStore.getLastBlocks),
            switchMap(blocks => {
                return of(blocks.map<BlockSummaryModel>((block, index) => BlockSummaryModel.fromBlockResponseModel(block)));
            })
        );

        if (this.autorefresh > 0) {
            interval(this.autorefresh * 1000).pipe(
                startWith(0),
                takeUntil(this.destroyed$)
            ).subscribe(() => {
                if (this.blocksLoaded$)
                    this.store.dispatch(fromStore.loadLastBlocks(this.records));
            });
        }
        else {
            this.store.dispatch(fromStore.loadLastBlocks(this.records));
        }
    }

    ngOnDestroy(): void {
        this.destroyed$.next(true);
        this.destroyed$.complete();
    }
}
