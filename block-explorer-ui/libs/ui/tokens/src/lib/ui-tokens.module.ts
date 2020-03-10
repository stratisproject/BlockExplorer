import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { Route, RouterModule } from '@angular/router';
import { UiLayoutModule } from '@blockexplorer/ui/layout';
import { UiSmartContractsModule } from '@blockexplorer/ui/smart-contracts';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { PrismModule } from '@ngx-prism/core';
import { ClipboardModule } from 'ngx-clipboard';


@NgModule({
  imports: [CommonModule, RouterModule, UiLayoutModule, NgbModule, ClipboardModule, PrismModule, UiSmartContractsModule],
  declarations: [],
  exports: []
})
export class UiTokensModule { }
