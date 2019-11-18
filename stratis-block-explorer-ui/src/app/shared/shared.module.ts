import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import * as components from './components';

@NgModule({
  declarations: [components.BalanceComponent, components.BusyIndicatorComponent, components.PagerComponent],
  imports: [
    CommonModule
  ],
  exports: [components.BalanceComponent, components.BusyIndicatorComponent, components.PagerComponent]
})
export class SharedModule { }

