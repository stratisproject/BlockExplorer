import { Component, OnInit, Input } from '@angular/core';
import { ReplaySubject, Observable, of } from 'rxjs';
import { BlockSummaryModel } from '../../models/block-summary.model';
import { select } from '@ngrx/store';
import { switchMap } from 'rxjs/operators';
import { BlocksFacade } from '../../store/blocks.facade';

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

    constructor(private blocksFacade: BlocksFacade) { }

    ngOnInit() {
        this.areBlocksLoaded$ = this.blocksFacade.loaded$;
        this.blocks$ = this.blocksFacade.blocks$
            .pipe(
                switchMap(blocks => {
                    return of(blocks.map<BlockSummaryModel>((block, index) => BlockSummaryModel.fromBlockResponseModel(block)));
                })
            );

        this.blocksFacade.loadBlocks(this.records);
    }

}
