import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { Route, RouterModule } from '@angular/router';
import { UiLayoutModule } from '@blockexplorer/ui/layout';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { PrismModule } from '@ngx-prism/core';
import { ClipboardModule } from 'ngx-clipboard';

import { SmartContractSummaryComponent } from './components/smart-contract-summary/smart-contract-summary.component';
import { SmartContractOpcodeComponent } from './components/smart-contract-opcode/smart-contract-opcode.component';
import { SmartContractSummaryPageComponent } from './containers/smart-contract-summary-page/smart-contract-summary-page.component';

export const uiSmartContractsRoutes: Route[] = [
  { path: ':addressHash', component: SmartContractSummaryPageComponent }
];

@NgModule({
  imports: [CommonModule, RouterModule, UiLayoutModule, NgbModule, PrismModule, ClipboardModule],
  declarations: [SmartContractSummaryComponent, SmartContractOpcodeComponent, SmartContractSummaryPageComponent],
  exports: [SmartContractSummaryComponent, SmartContractOpcodeComponent, SmartContractSummaryPageComponent]
})
export class UiSmartContractsModule { }
