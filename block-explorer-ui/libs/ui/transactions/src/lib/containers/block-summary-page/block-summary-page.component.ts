import { Component, OnInit, OnDestroy } from '@angular/core';
import { Observable, ReplaySubject } from 'rxjs';
import { BlockResponseModel, TransactionSummaryModel } from '@blockexplorer/shared/models';
import { ActivatedRoute } from '@angular/router';
import { TransactionsFacade } from '@blockexplorer/state/transactions-state';
import { takeUntil } from 'rxjs/operators';
import { Log } from '@blockexplorer/shared/utils';

@Component({
  selector: 'blockexplorer-block-summary-page',
  templateUrl: './block-summary-page.component.html',
  styleUrls: ['./block-summary-page.component.css']
})
export class BlockSummaryPageComponent implements OnInit, OnDestroy {
  blockLoaded$: Observable<boolean>;
  transactions: TransactionSummaryModel[] = [];
  destroyed$ = new ReplaySubject<any>();
  height = '';
  block$: Observable<BlockResponseModel>;

  constructor(private route: ActivatedRoute, private transactionsFacade: TransactionsFacade, private log: Log) { }

  ngOnInit() {
    this.route.paramMap
        .pipe(takeUntil(this.destroyed$))
        .subscribe((paramMap: any) => {
          if (!!paramMap.params.blockHeight) {
              this.height = paramMap.params.blockHeight;
              this.transactionsFacade.getBlock(this.height);
          }
        });
    this.loadBlockDetails();
  }

  private loadBlockDetails() {
    this.blockLoaded$ = this.transactionsFacade.loadedBlockData$;
    this.block$ = this.transactionsFacade.selectedBlock$;
    this.block$.pipe(takeUntil(this.destroyed$))
        .subscribe(block => {
          this.transactions.length = 0;
          this.log.info('Found block details', block);
          if (!block || !block.block || !block.block.transactions) return;

          this.transactions = block.block.transactions;
        });
  }

  ngOnDestroy(): void {
    this.destroyed$.next(true);
    this.destroyed$.complete();
  }
}
