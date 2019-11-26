import { Component, OnInit, Input } from '@angular/core';
import { ReplaySubject, Observable, of } from 'rxjs';
import { BlockSummaryModel } from '../../models/block-summary.model';
import { select } from '@ngrx/store';
import * as fromStore from '../../store';
import { switchMap } from 'rxjs/operators';
import { BlockStoreFacade } from '../../store/block-store.facade';

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

    constructor(private facade: BlockStoreFacade) { }

    ngOnInit() {
        this.areBlocksLoaded$ = this.facade.areBlocksLoaded$;
        this.blocks$ = this.facade.blocks$
            .pipe(
                switchMap(blocks => {
                    return of(blocks.map<BlockSummaryModel>((block, index) => BlockSummaryModel.fromBlockResponseModel(block)));
                })
            );

        this.facade.loadBlocks(this.records);
    }

}
