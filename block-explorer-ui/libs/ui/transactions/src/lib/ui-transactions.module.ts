import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { Route, RouterModule } from '@angular/router';
import { UiLayoutModule } from '@blockexplorer/ui/layout';
import { UiSmartContractsModule } from '@blockexplorer/ui/smart-contracts';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { PrismModule } from '@ngx-prism/core';
import { ClipboardModule } from 'ngx-clipboard';

import { AddressSummaryComponent } from './components/address-summary/address-summary.component';
import { BlockSummaryComponent } from './components/block-summary/block-summary.component';
import { LastBlocksComponent } from './components/last-blocks/last-blocks.component';
import { LastSmartContractsComponent } from './components/last-smart-contracts/last-smart-contracts.component';
import { StatsComponent } from './components/stats/stats.component';
import { TransactionListItemComponent } from './components/transaction-list-item/transaction-list-item.component';
import { TransactionSummaryComponent } from './components/transaction-summary/transaction-summary.component';
import { TransactionsListComponent } from './components/transactions-list/transactions-list.component';
import { TransactionsTableComponent } from './components/transactions-table/transactions-table.component';
import { AddressSummaryPageComponent } from './containers/address-summary-page/address-summary-page.component';
import { BlockSummaryPageComponent } from './containers/block-summary-page/block-summary-page.component';
import { NotFoundPageComponent } from './containers/not-found-page/not-found-page.component';
import { TransactionSummaryPageComponent } from './containers/transaction-summary-page/transaction-summary-page.component';
import { TransactionsPageComponent } from './containers/transactions-page/transactions-page.component';

export { TransactionsPageComponent } from './containers/transactions-page/transactions-page.component';

export const uiTransactionsRoutes: Route[] = [
  { path: '', component: TransactionsPageComponent },
  { path: ':hash', component: TransactionSummaryPageComponent }
];

export const uiAddressesRoutes: Route[] = [
  { path: ':addressHash', component: AddressSummaryPageComponent }
];

export const uiBlockRoutes: Route[] = [
  { path: ':blockHeight', component: BlockSummaryPageComponent }
];

export const uiOtherRoutes: Route[] = [
  { path: 'not-found', component: NotFoundPageComponent }
];

@NgModule({
  imports: [CommonModule, RouterModule, UiLayoutModule, NgbModule, ClipboardModule, PrismModule, UiSmartContractsModule],
  declarations: [TransactionsPageComponent, LastBlocksComponent, TransactionsListComponent, AddressSummaryComponent, TransactionListItemComponent, TransactionSummaryPageComponent, AddressSummaryPageComponent, TransactionSummaryComponent, NotFoundPageComponent, BlockSummaryComponent, BlockSummaryPageComponent, LastSmartContractsComponent, StatsComponent, TransactionsTableComponent],
  exports: [TransactionsPageComponent, TransactionSummaryPageComponent, AddressSummaryPageComponent, NotFoundPageComponent, BlockSummaryPageComponent, LastBlocksComponent, LastSmartContractsComponent, StatsComponent]
})
export class UiTransactionsModule { }
