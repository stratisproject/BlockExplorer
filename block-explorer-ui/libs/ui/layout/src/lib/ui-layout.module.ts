import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MenuBarComponent } from './components/menu-bar/menu-bar.component';
import { FormsModule } from '@angular/forms';
import { BreadcrumbComponent } from './components/breadcrumb/breadcrumb.component';
import { PagerComponent } from './components/pager/pager.component';
import { BalanceComponent } from './components/balance/balance.component';
import { BusyIndicatorComponent } from './components/busy-indicator/busy-indicator.component';

@NgModule({
  imports: [CommonModule, FormsModule],
  declarations: [MenuBarComponent, BreadcrumbComponent, PagerComponent, BalanceComponent, BusyIndicatorComponent],
  exports: [MenuBarComponent, BreadcrumbComponent, PagerComponent, BalanceComponent, BusyIndicatorComponent]
})
export class UiLayoutModule {}
