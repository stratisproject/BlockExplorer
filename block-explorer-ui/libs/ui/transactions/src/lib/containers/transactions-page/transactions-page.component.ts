import { Component, OnInit, OnDestroy } from '@angular/core';
import { TransactionsFacade } from '@blockexplorer/state/transactions-state';
import { Observable, ReplaySubject, interval } from 'rxjs';
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

  constructor(private transactionsFacade: TransactionsFacade, private log: Log) { }

  ngOnInit() {

    interval(10 * 1000).pipe(
      startWith(0),
      takeUntil(this.destroyed$)
    ).subscribe(() => {
      this.transactionsFacade.getLastBlocks();
    });

    this.blocksLoaded$ = this.transactionsFacade.lastBlocksLoaded$;
    this.blocks$ = this.transactionsFacade.lastBlocks$;
    this.blocks$.pipe(takeUntil(this.destroyed$))
        .subscribe(blocks => {
          this.blocks = blocks;
        });
  }

  selected(value: string){

  }

  get chain() {
    return APP_CONFIG.chain;
  }

  ngOnDestroy(): void {
    this.destroyed$.next(true);
    this.destroyed$.complete();
  }
}
