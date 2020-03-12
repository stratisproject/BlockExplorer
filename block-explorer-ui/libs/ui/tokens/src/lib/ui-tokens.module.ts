import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { Route, RouterModule } from '@angular/router';
import { UiLayoutModule } from '@blockexplorer/ui/layout';
import { UiSmartContractsModule } from '@blockexplorer/ui/smart-contracts';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { PrismModule } from '@ngx-prism/core';
import { ClipboardModule } from 'ngx-clipboard';
import { TokenSummaryPageComponent } from './containers/token-summary-page/token-summary-page.component';
import { TokenTransactionsTableComponent } from './components/token-transactions-table/token-transactions-table.component';
import { TokenSummaryComponent } from './components/token-summary/token-summary.component';

export const uiTokensRoutes: Route[] = [
  { path: ':address', component: TokenSummaryPageComponent }
];
@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    UiLayoutModule,
    NgbModule,
    ClipboardModule,
    PrismModule,
    UiSmartContractsModule
  ],
  declarations: [TokenSummaryPageComponent, TokenTransactionsTableComponent, TokenSummaryComponent],
  exports: [TokenSummaryPageComponent]
})
export class UiTokensModule {}
