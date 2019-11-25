import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import * as fromStore from '../../store/reducers';
import { Observable, interval, ReplaySubject, of } from 'rxjs';
import { Store, select } from '@ngrx/store';
import { startWith, takeUntil, switchMap, tap } from 'rxjs/operators';
import { BlockSummaryModel } from '../../models/block-summary.model';
import { BlockStoreFacade } from '../../store/block-store.facade';
import { takeUntilDestroyed } from '../../../../shared/shared.module';

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

    constructor(private facade: BlockStoreFacade) { }

    ngOnInit() {
        this.areLastBlockLoaded$ = this.facade.areLastBlocksLoaded$;
        this.blocks$ = this.facade.lastBlocks$.pipe(
            switchMap(blocks => {
                if (blocks == null) return of(null);
                return of(blocks.map<BlockSummaryModel>((block, index) => BlockSummaryModel.fromBlockResponseModel(block)));
            })
            ,takeUntilDestroyed(this)
        );

        if (this.autorefresh > 0) {
            interval(this.autorefresh * 1000)
                .pipe(
                    startWith(0),
                    takeUntilDestroyed(this)
                ).subscribe(() => {
                    if (this.areLastBlockLoaded$) {
                        this.facade.loadLastBlocks(this.records);
                    }
                });
        }
        else {
            this.facade.loadLastBlocks(this.records);
        }
    }

    ngOnDestroy(): void {
    }
}
