import { Component, OnInit, OnDestroy } from '@angular/core';
import { Observable, ReplaySubject, combineLatest } from 'rxjs';
import {
  TransactionSummaryModel,
  SmartContractModel
} from '@blockexplorer/shared/models';
import { ActivatedRoute } from '@angular/router';
import { takeUntil, map, filter } from 'rxjs/operators';
import { Log } from '@blockexplorer/shared/utils';
import { TokensFacade } from 'libs/state/tokens-state/src';
import { TokenTransactionResponse } from 'libs/state/tokens-state/src/lib/services/token-transaction-response';
import { TokenDetail } from 'libs/state/tokens-state/src/lib/services/token-detail';

@Component({
  selector: 'blockexplorer-token-summary-page',
  templateUrl: './token-summary-page.component.html',
  styleUrls: ['./token-summary-page.component.css']
})
export class TokenSummaryPageComponent implements OnInit, OnDestroy {
  transactionsLoaded$: Observable<boolean>;
  transactions: TokenTransactionResponse[] = [];
  destroyed$ = new ReplaySubject<any>();
  hash = '';
  filterAddress: string;
  transactions$: Observable<TokenTransactionResponse[]>;
  detailLoaded$: Observable<boolean>;
  selectedDetail$: Observable<TokenDetail>;
  tokenDetail: TokenDetail;
  balance$: Observable<number>;
  

  constructor(
    private route: ActivatedRoute,
    private tokensFacade: TokensFacade,
    private log: Log
  ) {}

  ngOnInit() {
    console.log(this.route.snapshot);
    combineLatest(
      this.route.paramMap,
      this.route.queryParamMap
    )
    .pipe(takeUntil(this.destroyed$))
    .subscribe(([paramMap, queryParamMap]) => {
      console.log(paramMap);
      console.log(queryParamMap);
      if (paramMap.has("address")) {
        this.hash = paramMap.get("address");
                
        this.tokensFacade.loadDetail(this.hash);

        if(queryParamMap.has("a")) {
          this.tokensFacade.loadAll(this.hash, queryParamMap.get("a"));
          this.filterAddress = queryParamMap.get("a");
        } 
        else {
          this.tokensFacade.loadRecent(this.hash);
          // This appears to be necessary when we change clear filter on the page
          this.filterAddress = undefined;
        }
      }
    })

    this.loadTokenDetails();
  }

  private loadTokenDetails() {
    this.transactionsLoaded$ = this.tokensFacade.loaded$;
    this.transactions$ = this.tokensFacade.allTokens$;
    this.transactions$.pipe(takeUntil(this.destroyed$))
        .subscribe(tokens => {
          this.transactions = tokens;
        });

    this.detailLoaded$ = this.tokensFacade.detailLoaded$;
    this.selectedDetail$ = this.tokensFacade.selectedDetail$;
    this.selectedDetail$.pipe(takeUntil(this.destroyed$))
        .subscribe(detail => {
          this.tokenDetail = detail;
        });

    this.balance$ = this.transactions$.pipe(
      takeUntil(this.destroyed$),
      filter(() => !!this.filterAddress),
      map(tokens => {
        return tokens
          .map(t => {
            if(t.fromAddress == this.filterAddress)
              return -t.amount;
            if(t.toAddress == this.filterAddress)
              return +t.amount;
            return 0;
          })
          .reduce((t, acc) => {
            return t + acc;
          }, 0);
      }))
  }

  ngOnDestroy(): void {
    this.destroyed$.next(true);
    this.destroyed$.complete();
  }
}
