import { Component, OnInit, OnDestroy } from '@angular/core';
import { Observable, BehaviorSubject, ReplaySubject, interval, Subject } from 'rxjs';
import { StatsModel } from '../../../block/models/stats.model';
import { TransactionSummaryModel } from '../../../transaction/models/transaction-summary.model';
import { BlocksFacade } from '../../../block/store/blocks.facade';
import { startWith, takeUntil } from 'rxjs/operators';
import { takeUntilDestroyed } from '@shared/shared.module';

@Component({
   selector: 'app-dashboard',
   templateUrl: './dashboard.component.html',
   styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit, OnDestroy {
   destroyed$ = new ReplaySubject<any>();
   stats$: Observable<StatsModel>;
   statsLoaded$: Observable<boolean>;
   statsError$: Observable<boolean>;

   statsRefresh: number = 30;

   constructor(private blockFacade: BlocksFacade) { }

   ngOnInit() {
      this.statsLoaded$ = this.blockFacade.statsLoaded$;
      this.stats$ = this.blockFacade.stats$;
      this.statsError$ = this.blockFacade.statsError$;

      interval(this.statsRefresh * 1000).pipe(
         startWith(0),
         takeUntil(this.destroyed$)
      ).subscribe(() => {
         this.blockFacade.getStats();
      });
   }

   ngOnDestroy(): void {
      this.destroyed$.next(true);
      this.destroyed$.complete();
   }
}
