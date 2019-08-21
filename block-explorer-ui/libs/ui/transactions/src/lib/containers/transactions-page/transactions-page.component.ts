import { Component, OnInit, OnDestroy } from '@angular/core';
import { TransactionsFacade } from '@blockexplorer/state/transactions-state';
import { Observable, ReplaySubject, interval, BehaviorSubject } from 'rxjs';
import { BlockResponseModel, APP_CONFIG, TransactionSummaryModel, StatsModel } from '@blockexplorer/shared/models';
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
  smartContracts$: Observable<TransactionSummaryModel[]>;
  smartContractsLoaded$: Observable<boolean>;
  smartContracts: TransactionSummaryModel[] = [];
  stats$: Observable<StatsModel>;
  statsLoaded$: Observable<boolean>;
  stats: StatsModel = null;
  destroyed$ = new ReplaySubject<any>();
  loadingMore$ = new BehaviorSubject<boolean>(false);
  loading = false;
  loadingMoreScs$ = new BehaviorSubject<boolean>(false);
  loadingScs = false;
  blockRecords = 10;
  scRecords = 10;

  constructor(private transactionsFacade: TransactionsFacade, private log: Log) { }

  ngOnInit() {

    interval(30 * 1000).pipe(
      startWith(0),
      takeUntil(this.destroyed$)
    ).subscribe(() => {
      this.transactionsFacade.getStats();
    });

    interval(40 * 1000).pipe(
      startWith(0),
      takeUntil(this.destroyed$)
    ).subscribe(() => {
      if (!this.loading)
        this.transactionsFacade.getLastBlocks(this.blockRecords);
    });

    interval(120 * 1000).pipe(
      startWith(0),
      takeUntil(this.destroyed$)
    ).subscribe(() => {
      if (!this.loadingScs)
        this.transactionsFacade.getLastSmartContracts(this.scRecords);
    });

    this.blocksLoaded$ = this.transactionsFacade.lastBlocksLoaded$;
    this.blocks$ = this.transactionsFacade.lastBlocks$;
    this.blocks$.pipe(takeUntil(this.destroyed$))
        .subscribe(blocks => {
          this.blocks = blocks;
          this.loading = false;
          this.loadingMore$.next(false);
        });

    this.smartContractsLoaded$ = this.transactionsFacade.loadedSmartContractTransactions$;
    this.smartContracts$ = this.transactionsFacade.smartContractTransactions$;
    this.smartContracts$.pipe(takeUntil(this.destroyed$))
        .subscribe(smartContracts => {
          this.smartContracts = smartContracts;
          this.loadingScs = false;
          this.loadingMoreScs$.next(false);
        });

    this.statsLoaded$ = this.transactionsFacade.loadedStats$;
    this.stats$ = this.transactionsFacade.stats$;
    this.stats$.pipe(takeUntil(this.destroyed$))
        .subscribe(stats => {
          this.stats = stats;
        });
  }

  selected(value: string){
  }

  get chain() {
    return APP_CONFIG.chain;
  }

  loadMore(records: number) {
    this.blockRecords = records;
    this.loading = true;
    this.loadingMore$.next(true);
    this.transactionsFacade.getLastBlocks(records);
  }

  loadMoreScs(records: number) {
    this.scRecords = records;
    this.loadingScs = true;
    this.loadingMoreScs$.next(true);
    this.transactionsFacade.getLastSmartContracts(records);
  }

  ngOnDestroy(): void {
    this.destroyed$.next(true);
    this.destroyed$.complete();
  }
}
