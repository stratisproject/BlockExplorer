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
import { BlockSummaryComponent } from './components/block-summary/block-summary.component';
import { BlockSummaryPageComponent } from './containers/block-summary-page/block-summary-page.component';
import { UiSmartContractsModule } from '@blockexplorer/ui/smart-contracts';
import { LastBlocksComponent } from './components/last-blocks/last-blocks.component';
import { LastSmartContractsComponent } from './components/last-smart-contracts/last-smart-contracts.component';
import { StatsComponent } from './components/stats/stats.component';

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
  imports: [CommonModule, RouterModule, UiLayoutModule, NgbModule, PrismModule, UiSmartContractsModule],
  declarations: [TransactionsPageComponent, LastBlocksComponent, TransactionsListComponent, AddressSummaryComponent, TransactionListItemComponent, TransactionSummaryPageComponent, AddressSummaryPageComponent, TransactionSummaryComponent, NotFoundPageComponent, BlockSummaryComponent, BlockSummaryPageComponent, LastSmartContractsComponent, StatsComponent],
  exports: [TransactionsPageComponent, TransactionSummaryPageComponent, AddressSummaryPageComponent, NotFoundPageComponent, BlockSummaryPageComponent, LastBlocksComponent, LastSmartContractsComponent, StatsComponent]
})
export class UiTransactionsModule {}
