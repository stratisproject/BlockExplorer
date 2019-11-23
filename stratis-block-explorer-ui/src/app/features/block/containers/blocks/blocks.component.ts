import { Component, OnInit, Input } from '@angular/core';
import { ReplaySubject, Observable, of } from 'rxjs';
import { BlockSummaryModel } from '../../models/block-summary.model';
import { select, Store } from '@ngrx/store';
import * as fromStore from '../../store';
import { switchMap } from 'rxjs/operators';

@Component({
    selector: 'app-blocks',
    templateUrl: './blocks.component.html',
    styleUrls: ['./blocks.component.scss']
})
export class BlocksComponent implements OnInit {
    destroyed$ = new ReplaySubject<boolean>();
    displayedColumns = ['height', 'age', 'confirmations', 'transactions', 'hash'];

    areBlocksLoaded$: Observable<boolean>;
    blocks$: Observable<BlockSummaryModel[]>;

    @Input() records: number = 25;

    constructor(private store: Store<fromStore.BlockState>) { }

    ngOnInit() {
        this.areBlocksLoaded$ = this.store.pipe(select(fromStore.getAreBlocksLoaded));
        this.blocks$ = this.store.pipe(
            select(fromStore.getBlocks),
            switchMap(blocks => {
                return of(blocks.map<BlockSummaryModel>((block, index) => BlockSummaryModel.fromBlockResponseModel(block)));
            })
        );

        this.store.dispatch(fromStore.loadBlocks(this.records));
    }

}
