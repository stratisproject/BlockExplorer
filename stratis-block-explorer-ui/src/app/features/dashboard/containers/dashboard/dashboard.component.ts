import { Component, OnInit } from '@angular/core';
import { Observable, BehaviorSubject, ReplaySubject } from 'rxjs';
import { StatsModel } from '../../../block/models/stats.model';
import { Log } from '@shared/logger.service';
import { TransactionSummaryModel } from '../../../transaction/models/transaction-summary.model';

@Component({
   selector: 'app-dashboard',
   templateUrl: './dashboard.component.html',
   styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
   smartContracts$: Observable<TransactionSummaryModel[]>;
   smartContractsLoaded$: Observable<boolean>;
   smartContracts: TransactionSummaryModel[] = [];
   stats$: Observable<StatsModel>;
   statsLoaded$: Observable<boolean>;
   stats: StatsModel = null;
   destroyed$ = new ReplaySubject<any>();
   loading = false;
   loadingMoreScs$ = new BehaviorSubject<boolean>(false);
   loadingScs = false;
   blockRecords = 10;
   scRecords = 10;

   constructor(private log: Log) { }

   ngOnInit() {
      //interval(30 * 1000).pipe(
      //   startWith(0),
      //   takeUntil(this.destroyed$)
      //).subscribe(() => {
      //   this.store.dispatch(fromBlockAction.LoadStats());
      //});

      //interval(120 * 1000).pipe(
      //   startWith(0),
      //   takeUntil(this.destroyed$)
      //).subscribe(() => {
      //   if (!this.loadingScs)
      //      this.transactionsFacade.getLastSmartContracts(this.scRecords);
      //});


      //this.smartContractsLoaded$ = this.transactionsFacade.loadedSmartContractTransactions$;
      //this.smartContracts$ = this.transactionsFacade.smartContractTransactions$;
      //this.smartContracts$.pipe(takeUntil(this.destroyed$))
      //   .subscribe(smartContracts => {
      //      this.smartContracts = smartContracts;
      //      this.loadingScs = false;
      //      this.loadingMoreScs$.next(false);
      //   });

      //this.statsLoaded$ = this.transactionsFacade.loadedStats$;
      //this.stats$ = this.transactionsFacade.stats$;
      //this.stats$.pipe(takeUntil(this.destroyed$))
      //   .subscribe(stats => {
      //      this.stats = stats;
      //   });
   }
}
