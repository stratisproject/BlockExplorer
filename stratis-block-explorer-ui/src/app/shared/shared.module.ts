import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import * as components from './components';
import { MatIconModule } from '@angular/material/icon';
import { CustomMaterialModuleModule } from './custom-material-module.module';
import { FlexLayoutModule } from '@angular/flex-layout';
import { ClipboardModule } from 'ngx-clipboard';
import { AddressViewComponent } from './components/address-view/address-view.component';
import { RouterModule } from '@angular/router';

@NgModule({
    declarations: [components.BalanceComponent, components.BusyIndicatorComponent, components.PagerComponent, components.HashViewComponent, components.AddressViewComponent],
    imports: [
        MatIconModule,
        CustomMaterialModuleModule,
        CommonModule,
        RouterModule,
        FlexLayoutModule,
        ClipboardModule
    ],
    exports: [
        components.BalanceComponent, components.BusyIndicatorComponent, components.PagerComponent, components.HashViewComponent, components.AddressViewComponent,
        CustomMaterialModuleModule,
        FlexLayoutModule,
        ClipboardModule
    ]
})
export class SharedModule { }

export { takeUntilDestroyed } from './rxjs/operators/take-until-destroyed';
