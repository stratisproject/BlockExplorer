import { Component, OnInit, OnDestroy } from '@angular/core';
import { Observable, ReplaySubject } from 'rxjs';
import { TransactionSummaryModel, SmartContractModel } from '@blockexplorer/shared/models';
import { ActivatedRoute } from '@angular/router';
import { takeUntil } from 'rxjs/operators';
import { Log } from '@blockexplorer/shared/utils';
import { TokensFacade } from 'libs/state/tokens-state/src';

@Component({
  selector: 'blockexplorer-token-summary-page',
  templateUrl: './token-summary-page.component.html',
  styleUrls: ['./token-summary-page.component.css']
})
export class TokenSummaryPageComponent implements OnInit, OnDestroy {
  transactionsLoaded$: Observable<boolean>;
  transactions: TransactionSummaryModel[] = [];
  smartContract: SmartContractModel = null;
  destroyed$ = new ReplaySubject<any>();
  hash = '';
  transaction$: Observable<TransactionSummaryModel>;
  isSmartContract = false;

  constructor(private route: ActivatedRoute, private tokensFacade: TokensFacade, private log: Log) { }

  ngOnInit() {
    this.route.paramMap
        .pipe(takeUntil(this.destroyed$))
        .subscribe((paramMap: any) => {
          if (!!paramMap.params.address) {
              this.hash = paramMap.params.address;
              console.log(this.hash);
              //this.tokensFacade.getTransaction(this.hash);
          }
        });
    // this.loadTransactionDetails();
  }

  private loadTransactionDetails() {
    // this.transactionsLoaded$ = this.transactionsFacade.loadedTransactions$;
    // this.transaction$ = this.transactionsFacade.selectedTransaction$;
    // this.transaction$.pipe(takeUntil(this.destroyed$))
    //     .subscribe(transaction => {
    //       this.isSmartContract = false;
    //       this.smartContract = null;
    //       this.transactions.length = 0;
    //       this.log.info('Found transaction details', transaction);
    //       if (!transaction) return;

    //       this.transactions = [transaction];
    //       if (!!transaction.smartContract) {
    //         this.isSmartContract = true;
    //         this.smartContract = transaction.smartContract;
    //       }
    //     });
  }

  ngOnDestroy(): void {
    this.destroyed$.next(true);
    this.destroyed$.complete();
  }
}
