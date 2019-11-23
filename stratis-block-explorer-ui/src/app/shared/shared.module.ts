import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import * as components from './components';
import { MatIconModule } from '@angular/material/icon';
import { CustomMaterialModuleModule } from './custom-material-module.module';
import { FlexLayoutModule } from '@angular/flex-layout';

@NgModule({
   declarations: [components.BalanceComponent, components.BusyIndicatorComponent, components.PagerComponent],
   imports: [
      MatIconModule,
      CustomMaterialModuleModule,
      CommonModule,
      FlexLayoutModule
   ],
   exports: [
      components.BalanceComponent, components.BusyIndicatorComponent, components.PagerComponent,
      CustomMaterialModuleModule,
      FlexLayoutModule
   ]
})
export class SharedModule { }

export { takeUntilDestroyed } from './rxjs/operators/take-until-destroyed';
