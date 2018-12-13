import { Component, OnInit, OnDestroy } from '@angular/core';
import { ReplaySubject, Observable } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { takeUntil, map } from 'rxjs/operators';
import { BalanceSummaryModel, BalanceResponseModel, TransactionSummaryModel } from '@blockexplorer/shared/models';
import { TransactionsFacade } from '@blockexplorer/state/transactions-state';

@Component({
  selector: 'blockexplorer-address-summary-page',
  templateUrl: './address-summary-page.component.html',
  styleUrls: ['./address-summary-page.component.css']
})
export class AddressSummaryPageComponent implements OnInit, OnDestroy {
  addressSummaryLoaded$: Observable<boolean>;
  lastTransaction: TransactionSummaryModel[] = [];
  transactions: TransactionSummaryModel[] = [];
  addressDetailsLoaded$: Observable<boolean>;
  destroyed$ = new ReplaySubject<any>();
  addressHash = '';
  address$: Observable<BalanceSummaryModel>;
  addressDetails$: Observable<BalanceResponseModel>;
  code = `
using OpenQA.Selenium;

namespace Stratis.Tests.UI.NetCore.Pages
{
    public class BasePage
    {
        public IWebDriver Driver { get; }

        public BasePage(IWebDriver driver)
        {
            this.Driver = driver;
        }

    }
}`;

  constructor(private route: ActivatedRoute, private transactionsFacade: TransactionsFacade) { }

  ngOnInit() {
    this.route.paramMap
        .pipe(takeUntil(this.destroyed$))
        .subscribe((paramMap: any) => {
          if (!!paramMap.params.addressHash) {
              this.addressHash = paramMap.params.addressHash;
              this.transactionsFacade.getAddress(this.addressHash);
              this.transactionsFacade.getAddressDetails(this.addressHash);
          }
        });

    this.loadAddressSummary();
    this.loadAddressDetails();
  }

  private loadAddressDetails() {
    this.addressDetailsLoaded$ = this.transactionsFacade.loadedAddressDetails$;
    this.addressDetails$ = this.transactionsFacade.selectedAddressDetails$;
    this.addressDetails$.pipe(takeUntil(this.destroyed$))
        .subscribe(addressDetails => {
          if (!addressDetails || !addressDetails.operations) return;
          this.transactions.length = 0;
          this.lastTransaction.length = 0;
          this.transactions = addressDetails.operations.map(op => op.transactionSummary);
          this.lastTransaction = this.transactions.length > 0 ? [this.transactions[0]] : [];

          console.log('Found address details', addressDetails);
        });
  }

  private loadAddressSummary() {
    this.addressSummaryLoaded$ = this.transactionsFacade.loadedAddress$;
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
