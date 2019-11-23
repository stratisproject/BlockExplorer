import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Route } from '@angular/router';
import * as fromContainers from './containers';
import { SharedModule } from '../../shared/shared.module';
import { BlockModule } from '../block/block.module';

export const dashboardRoutes: Route[] = [
   { path: '', component: fromContainers.DashboardComponent, data: { breadcrumb: 'Dashboard' } }
];

@NgModule({
   declarations: [fromContainers.DashboardComponent],
   imports: [
      CommonModule,
      SharedModule,
      BlockModule
   ]
})
export class DashboardModule { }
