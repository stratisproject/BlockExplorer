import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MenuBarComponent } from './components/menu-bar/menu-bar.component';
import { FormsModule } from '@angular/forms';
import { BreadcrumbComponent } from './components/breadcrumb/breadcrumb.component';

@NgModule({
  imports: [CommonModule, FormsModule],
  declarations: [MenuBarComponent, BreadcrumbComponent],
  exports: [MenuBarComponent, BreadcrumbComponent]
})
export class UiLayoutModule {}
