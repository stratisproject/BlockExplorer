import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { Route, RouterModule } from '@angular/router';
import { UiLayoutModule } from '@blockexplorer/ui/layout';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { PrismModule } from '@ngx-prism/core';

import { SmartContractSummaryComponent } from './components/smart-contract-summary/smart-contract-summary.component';

export const uiSmartContractsRoutes: Route[] = [];

@NgModule({
  imports: [CommonModule, RouterModule, UiLayoutModule, NgbModule, PrismModule],
  declarations: [SmartContractSummaryComponent],
  exports: [SmartContractSummaryComponent]
})
export class UiSmartContractsModule { }
