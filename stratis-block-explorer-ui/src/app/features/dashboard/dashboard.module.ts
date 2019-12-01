import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Route, RouterModule } from '@angular/router';
import * as fromContainers from './containers';
import { SharedModule } from '../../shared/shared.module';
import { BlockModule } from '../block/block.module';
import { TransactionModule } from '../transaction/transaction.module';

export const dashboardRoutes: Route[] = [
    { path: '', component: fromContainers.DashboardComponent, data: { breadcrumb: 'Dashboard' } }
];

@NgModule({
    declarations: [fromContainers.DashboardComponent],
    imports: [
        RouterModule.forChild(dashboardRoutes),
        CommonModule,
        SharedModule,
        BlockModule,
        TransactionModule
    ]
})
export class DashboardModule { }
