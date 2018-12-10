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

export { TransactionsPageComponent } from './containers/transactions-page/transactions-page.component';

export const uiTransactionsRoutes: Route[] = [
  { path: '', component: TransactionsPageComponent },
  { path: ':transactionHash', component: TransactionSummaryPageComponent }
];

export const uiAddressesRoutes: Route[] = [
  { path: ':addressHash', component: AddressSummaryPageComponent }
];

@NgModule({
  imports: [CommonModule, RouterModule, UiLayoutModule],
  declarations: [TransactionsPageComponent, TransactionsListComponent, AddressSummaryComponent, TransactionListItemComponent, TransactionSummaryPageComponent, AddressSummaryPageComponent],
  exports: [TransactionsPageComponent, TransactionSummaryPageComponent, AddressSummaryPageComponent]
})
export class UiTransactionsModule {}
