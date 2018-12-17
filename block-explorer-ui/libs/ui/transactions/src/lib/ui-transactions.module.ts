import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Route } from '@angular/router';
import { TransactionsPageComponent } from './containers/transactions-page/transactions-page.component';
import { TransactionsListComponent } from './components/transactions-list/transactions-list.component';
import { UiLayoutModule } from '@blockexplorer/ui/layout';
import { AddressSummaryComponent } from './components/address-summary/address-summary.component';
import { TransactionListItemComponent } from './components/transaction-list-item/transaction-list-item.component';
import { TransactionSummaryPageComponent } from './containers/transaction-summary-page/transaction-summary-page.component';
import { AddressSummaryPageComponent } from './containers/address-summary-page/address-summary-page.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { PrismModule } from '@ngx-prism/core';
import { TransactionSummaryComponent } from './components/transaction-summary/transaction-summary.component';
import { NotFoundPageComponent } from './containers/not-found-page/not-found-page.component';

export { TransactionsPageComponent } from './containers/transactions-page/transactions-page.component';

export const uiTransactionsRoutes: Route[] = [
  { path: '', component: TransactionsPageComponent },
  { path: ':hash', component: TransactionSummaryPageComponent }
];

export const uiAddressesRoutes: Route[] = [
  { path: ':addressHash', component: AddressSummaryPageComponent }
];

export const uiOtherRoutes: Route[] = [
  { path: 'not-found', component: NotFoundPageComponent }
];

@NgModule({
  imports: [CommonModule, RouterModule, UiLayoutModule, NgbModule, PrismModule],
  declarations: [TransactionsPageComponent, TransactionsListComponent, AddressSummaryComponent, TransactionListItemComponent, TransactionSummaryPageComponent, AddressSummaryPageComponent, TransactionSummaryComponent, NotFoundPageComponent],
  exports: [TransactionsPageComponent, TransactionSummaryPageComponent, AddressSummaryPageComponent, NotFoundPageComponent]
})
export class UiTransactionsModule {}
