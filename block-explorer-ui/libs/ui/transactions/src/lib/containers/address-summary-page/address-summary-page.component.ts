import { Component, OnInit, OnDestroy } from '@angular/core';
import { ReplaySubject, Observable } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { takeUntil } from 'rxjs/operators';
import { BalanceSummaryModel, BalanceResponseModel } from '@blockexplorer/shared/models';
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
    this.route.paramMap
        .pipe(takeUntil(this.destroyed$))
        .subscribe((map: any) => {
          if (!!map.params.addressHash) {
              this.addressHash = map.params.addressHash;
              this.transactionsFacade.getAddress(this.addressHash);
          }
        });
    this.address$ = this.transactionsFacade.selectedAddress$;
    this.address$.pipe(takeUntil(this.destroyed$))
        .subscribe(address => {
          if (!address) return;

          console.log('Found address', address);
        });
  }

  ngOnDestroy(): void {
    this.destroyed$.next(true);
    this.destroyed$.complete();
  }
}
