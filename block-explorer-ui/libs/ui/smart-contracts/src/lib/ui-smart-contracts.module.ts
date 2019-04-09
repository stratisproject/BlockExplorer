import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Route } from '@angular/router';
import { SmartContractSummaryComponent } from './components/smart-contract-summary/smart-contract-summary.component';
import { UiLayoutModule } from '@blockexplorer/ui/layout';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { PrismModule } from '@ngx-prism/core';

export const uiSmartContractsRoutes: Route[] = [];

@NgModule({
  imports: [CommonModule, RouterModule, UiLayoutModule, NgbModule, PrismModule],
  declarations: [SmartContractSummaryComponent],
  exports: [SmartContractSummaryComponent]
})
export class UiSmartContractsModule {}
