import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { Route, RouterModule } from '@angular/router';
import { UiLayoutModule } from '@blockexplorer/ui/layout';
import { UiSmartContractsModule } from '@blockexplorer/ui/smart-contracts';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { PrismModule } from '@ngx-prism/core';
import { ClipboardModule } from 'ngx-clipboard';
import { TokenSummaryPageComponent } from './containers/token-summary-page/token-summary-page.component';

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
  declarations: [TokenSummaryPageComponent],
  exports: [TokenSummaryPageComponent]
})
export class UiTokensModule {}
