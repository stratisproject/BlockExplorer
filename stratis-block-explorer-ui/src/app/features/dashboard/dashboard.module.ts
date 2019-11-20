import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Route } from '@angular/router';
import * as fromComponents from './components';

export const dashboardRoutes: Route[] = [
   { path: '', component: fromComponents.DashboardComponent, data: { breadcrumb: 'Dashboard' } }
];

@NgModule({
   declarations: [fromComponents.DashboardComponent],
   imports: [
      CommonModule
   ]
})
export class DashboardModule { }
