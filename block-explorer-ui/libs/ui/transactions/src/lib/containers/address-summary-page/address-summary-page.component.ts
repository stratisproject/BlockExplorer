import { Component, OnInit, OnDestroy } from '@angular/core';
import { ReplaySubject, Observable } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { takeUntil } from 'rxjs/operators';
import { BalanceSummaryModel } from '@blockexplorer/shared/models';
import { TransactionsFacade } from '@blockexplorer/state/transactions-state';

@Component({
  selector: 'blockexplorer-address-summary-page',
  templateUrl: './address-summary-page.component.html',
  styleUrls: ['./address-summary-page.component.css']
})
export class AddressSummaryPageComponent implements OnInit, OnDestroy {

  destroyed$ = new ReplaySubject<any>();
  addressHash = '';
  address$: Observable<BalanceSummaryModel>;

  constructor(private route: ActivatedRoute, private transactionsFacade: TransactionsFacade) { }

  ngOnInit() {
    this.route.queryParams
        .pipe(takeUntil(this.destroyed$))
        .subscribe((queryParams: any) => {
          if (!!queryParams.addressHash) {
              this.addressHash = queryParams.addressHash;
              this.transactionsFacade.getAddress(this.addressHash);
          }
        });
    this.address$ = this.transactionsFacade.selectedAddress$;
  }

  ngOnDestroy(): void {
    this.destroyed$.next(true);
    this.destroyed$.complete();
  }
}
