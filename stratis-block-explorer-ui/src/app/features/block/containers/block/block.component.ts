//import { Component, OnInit, OnDestroy } from '@angular/core';
//import { BlockResponseModel } from '../../models/block-response.model';
//import { Observable, ReplaySubject } from 'rxjs';
//import { TransactionSummaryModel } from '@shared/models/transaction-summary.model';
//import { ActivatedRoute } from '@angular/router';
//import { Store, select } from '@ngrx/store';
//import * as fromStore from '../../store';
//import { takeUntil, tap } from 'rxjs/operators';
//import { Log } from '@shared/logger.service';

//@Component({
//    selector: 'app-block',
//    templateUrl: './block.component.html',
//    styleUrls: ['./block.component.scss']
//})
//export class BlockComponent implements OnInit, OnDestroy {
//    isSelectedBlockLoaded$: Observable<boolean>;
//    destroyed$ = new ReplaySubject<any>();
//    hash = '';
//    selectedBlock$: Observable<BlockResponseModel>;

//    constructor(private route: ActivatedRoute, private store: Store<fromStore.BlockState>, private log: Log) { }

//    ngOnInit() {
//        this.loadBlockDetails();
//    }

//    private loadBlockDetails() {
//        this.route.paramMap.pipe(takeUntil(this.destroyed$))
//            .subscribe((paramMap: any) => {
//                if (!!paramMap.params.blockHash) {
//                    this.hash = paramMap.params.blockHash;
//                    this.store.dispatch(fromStore.loadBlock(this.hash));
//                }
//            });

//        this.isSelectedBlockLoaded$ = this.store.pipe(select(fromStore.getIsSelectedBlockLoaded));
//        this.selectedBlock$ = this.store.pipe(select(fromStore.getSelectedBlock));
//    }

//    ngOnDestroy(): void {
//        this.destroyed$.next(true);
//        this.destroyed$.complete();
//    }
//}


import { Component, OnInit, OnDestroy } from '@angular/core';
import { BlockResponseModel } from '../../models/block-response.model';
import { Observable, ReplaySubject, timer } from 'rxjs';
import { TransactionSummaryModel } from '@shared/models/transaction-summary.model';
import { ActivatedRoute } from '@angular/router';
import { Store, select } from '@ngrx/store';
import * as fromStore from '../../store';
import { takeUntil, tap } from 'rxjs/operators';
import { Log } from '@shared/logger.service';
import { takeUntilDestroyed } from '@shared/shared.module';

@Component({
    selector: 'app-block',
    templateUrl: './block.component.html',
    styleUrls: ['./block.component.scss']
})
export class BlockComponent implements OnInit, OnDestroy {
    isSelectedBlockLoaded$: Observable<boolean>;
    hash = '';
    selectedBlock$: Observable<BlockResponseModel>;

    constructor(private route: ActivatedRoute, private store: Store<fromStore.BlockState>, private log: Log) { }

    ngOnInit() {
        this.loadBlockDetails();
    }

    private loadBlockDetails() {
        this.route.paramMap.pipe(takeUntilDestroyed(this))
            .subscribe((paramMap: any) => {
                if (!!paramMap.params.blockHash) {
                    this.hash = paramMap.params.blockHash;
                    this.store.dispatch(fromStore.loadBlock(this.hash));
                }
            });

        this.isSelectedBlockLoaded$ = this.store.pipe(select(fromStore.getIsSelectedBlockLoaded));
        this.selectedBlock$ = this.store.pipe(select(fromStore.getSelectedBlock));
    }

    ngOnDestroy(): void {
    }
}
