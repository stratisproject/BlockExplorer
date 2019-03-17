import { Component, OnInit, OnDestroy } from '@angular/core';
import { TransactionsFacade } from '@blockexplorer/state/transactions-state';
import { Observable, ReplaySubject } from 'rxjs';
import { BlockResponseModel } from '@blockexplorer/shared/models';
import { Log } from '@blockexplorer/shared/utils';
import { takeUntil } from 'rxjs/operators';

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

  constructor(private transactionsFacade: TransactionsFacade, private log: Log) { }

  ngOnInit() {
    // this.transactions$ = this.transactionsFacade.allTransactions$;
    this.transactionsFacade.getLastBlocks();
    this.blocksLoaded$ = this.transactionsFacade.lastBlocksLoaded$;
    this.blocks$ = this.transactionsFacade.lastBlocks$;
    this.blocks$.pipe(takeUntil(this.destroyed$))
        .subscribe(blocks => {
          this.blocks = blocks;
        });
  }

  selected(value: string){

  }

  ngOnDestroy(): void {
    this.destroyed$.next(true);
    this.destroyed$.complete();
  }
}
