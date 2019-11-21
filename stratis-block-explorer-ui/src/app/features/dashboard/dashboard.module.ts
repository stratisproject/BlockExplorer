import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Route } from '@angular/router';
import * as fromComponents from './components';
import { SharedModule } from '../../shared/shared.module';
import { BlockModule } from '../block/block.module';

export const dashboardRoutes: Route[] = [
   { path: '', component: fromComponents.DashboardComponent, data: { breadcrumb: 'Dashboard' } }
];

@NgModule({
   declarations: [fromComponents.DashboardComponent],
   imports: [
      CommonModule,
      SharedModule,
      BlockModule
   ]
})
export class DashboardModule { }
