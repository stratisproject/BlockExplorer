import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { Observable, interval, of } from 'rxjs';
import { startWith, switchMap } from 'rxjs/operators';
import { BlockSummaryModel } from '../../models/block-summary.model';
import { LastBlocksFacade } from '../../store/last-blocks.facade';
import { takeUntilDestroyed } from '@shared/shared.module';

@Component({
    selector: 'app-latest-blocks',
    templateUrl: './latest-blocks.component.html',
    styleUrls: ['./latest-blocks.component.scss']
})
export class LatestBlocksComponent implements OnInit, OnDestroy {
    @Input() records: number = 0;
    @Input() autorefresh: number = 0;

    displayedColumns = ['height', 'age', 'confirmations', 'transactions', 'hash'];

    areLastBlockLoaded$: Observable<boolean>;
    blocks$: Observable<BlockSummaryModel[]>;

    constructor(private lastBlocksFacade: LastBlocksFacade) { }

    ngOnInit() {
        this.areLastBlockLoaded$ = this.lastBlocksFacade.loaded$;
        this.blocks$ = this.lastBlocksFacade.blocks$.pipe(
            switchMap(blocks => {
                if (blocks == null) return of(null);
                return of(blocks.map<BlockSummaryModel>((block, index) => BlockSummaryModel.fromBlockResponseModel(block)));
            })
            , takeUntilDestroyed(this)
        );

        if (this.autorefresh > 0) {
            interval(this.autorefresh * 1000)
                .pipe(
                    startWith(0),
                    takeUntilDestroyed(this)
                ).subscribe(() => {
                    if (this.areLastBlockLoaded$) {
                        this.lastBlocksFacade.loadLastBlocks(this.records);
                    }
                });
        }
        else {
            this.lastBlocksFacade.loadLastBlocks(this.records);
        }
    }

    ngOnDestroy(): void {
    }
}
