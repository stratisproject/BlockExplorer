import { Component, OnInit, OnDestroy } from '@angular/core';
import { TransactionsFacade } from '@blockexplorer/state/transactions-state';
import { Observable, ReplaySubject, interval, BehaviorSubject } from 'rxjs';
import { BlockResponseModel, APP_CONFIG } from '@blockexplorer/shared/models';
import { Log } from '@blockexplorer/shared/utils';
import { takeUntil, startWith, switchMap } from 'rxjs/operators';

@Component({
  selector: 'blockexplorer-transactions-page',
  templateUrl: './transactions-page.component.html',
  styleUrls: ['./transactions-page.component.css']
})
export class TransactionsPageComponent implements OnInit, OnDestroy {
  blocks$: Observable<BlockResponseModel[]>;
  blocksLoaded$: Observable<boolean>;
  blocks: BlockResponseModel[] = [];
  destroyed$ = new ReplaySubject<any>();
  records = 10;
  loadingMore$ = new BehaviorSubject<boolean>(false);
  loading = false;

  constructor(private transactionsFacade: TransactionsFacade, private log: Log) { }

  ngOnInit() {

    interval(40 * 1000).pipe(
      startWith(0),
      takeUntil(this.destroyed$)
    ).subscribe(() => {
      if (!this.loading)
        this.transactionsFacade.getLastBlocks(this.records);
    });

    this.blocksLoaded$ = this.transactionsFacade.lastBlocksLoaded$;
    this.blocks$ = this.transactionsFacade.lastBlocks$;
    this.blocks$.pipe(takeUntil(this.destroyed$))
        .subscribe(blocks => {
          this.blocks = blocks;
          this.loading = false;
          this.loadingMore$.next(false);
        });
  }

  selected(value: string){

  }

  get chain() {
    return APP_CONFIG.chain;
  }

  loadMore(records: number) {
    this.records = records;
    this.loading = true;
    this.loadingMore$.next(true);
    this.transactionsFacade.getLastBlocks(this.records);
  }

  ngOnDestroy(): void {
    this.destroyed$.next(true);
    this.destroyed$.complete();
  }
}
