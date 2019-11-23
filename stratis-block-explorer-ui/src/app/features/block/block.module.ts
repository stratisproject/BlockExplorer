import { NgModule, Type } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Route, RouterModule } from '@angular/router';
import * as fromComponents from './components';
import * as fromContainers from './containers';
import * as fromStore from './store';
import { SharedModule } from '@shared/shared.module';
import { StoreModule } from '@ngrx/store';
import { EffectsModule } from '@ngrx/effects';
import { BlockTransactionsComponent } from './components/block-transactions/block-transactions.component';
import { BlockTransactionsItemComponent } from './components/block-transactions-item/block-transactions-item.component';

export const blockRoutes: Route[] = [
    { path: 'blocks', component: fromContainers.BlocksComponent, data: { breadcrumb: 'Blocks' } },
    {
        path: 'block', data: { breadcrumb: 'Block' }, children: [
            { path: '', redirectTo: '/blocks', pathMatch: "full" },
            { path: ':blockHash', component: fromContainers.BlockComponent }
        ]
    }
];

const exportedComponents: Type<any>[] = [
    fromContainers.BlockComponent,
    fromContainers.BlocksComponent,
    fromComponents.LatestBlocksComponent,
    fromComponents.BlockSummaryComponent
];

@NgModule({
    declarations: [...exportedComponents, BlockTransactionsComponent, BlockTransactionsItemComponent],
    imports: [
        RouterModule,
        CommonModule,
        SharedModule,
        StoreModule.forFeature(fromStore.blockFeatureKey, fromStore.reducer),
        EffectsModule.forFeature([fromStore.BlockEffects])
    ],
    exports: [...exportedComponents]
})
export class BlockModule { }

