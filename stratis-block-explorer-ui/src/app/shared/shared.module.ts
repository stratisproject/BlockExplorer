import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import * as components from './components';
import { MatIconModule } from '@angular/material/icon';
import { CustomMaterialModuleModule } from './custom-material-module.module';
import { FlexLayoutModule } from '@angular/flex-layout';
import { ClipboardModule } from 'ngx-clipboard';
import { RouterModule } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';

@NgModule({
    declarations: [
        components.BalanceComponent,
        components.BusyIndicatorComponent,
        components.HashViewComponent,
        components.AddressViewComponent,
        components.MatAnimatedIconComponent,
        components.FaAnimatedIconComponent
    ],
    imports: [
        MatIconModule,
        CustomMaterialModuleModule,
        CommonModule,
        RouterModule,
        FlexLayoutModule,
        ClipboardModule,
        FontAwesomeModule
    ],
    exports: [
        components.BalanceComponent,
        components.BusyIndicatorComponent,
        components.HashViewComponent,
        components.AddressViewComponent,
        components.MatAnimatedIconComponent,
        components.FaAnimatedIconComponent,
        CustomMaterialModuleModule,
        FlexLayoutModule,
        ClipboardModule,
        FontAwesomeModule
    ]
})
export class SharedModule { }

export { takeUntilDestroyed } from './rxjs/operators/take-until-destroyed';
