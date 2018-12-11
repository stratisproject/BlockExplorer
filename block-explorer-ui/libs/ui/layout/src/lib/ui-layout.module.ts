import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MenuBarComponent } from './components/menu-bar/menu-bar.component';
import { FormsModule } from '@angular/forms';
import { BreadcrumbComponent } from './components/breadcrumb/breadcrumb.component';
import { PagerComponent } from './components/pager/pager.component';
import { BalanceComponent } from './components/balance/balance.component';

@NgModule({
  imports: [CommonModule, FormsModule],
  declarations: [MenuBarComponent, BreadcrumbComponent, PagerComponent, BalanceComponent],
  exports: [MenuBarComponent, BreadcrumbComponent, PagerComponent, BalanceComponent]
})
export class UiLayoutModule {}
