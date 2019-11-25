import { NgModule, Type } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import * as fromComponents from './components';
import * as fromContainers from './containers';
import * as fromStore from './store/reducers';
import { SharedModule } from '@shared/shared.module';
import { StoreModule } from '@ngrx/store';
import { EffectsModule } from '@ngrx/effects';
import { BlockTransactionsComponent } from './components/block-transactions/block-transactions.component';
import { BlockTransactionsItemComponent } from './components/block-transactions-item/block-transactions-item.component';
import { BlockEffects } from './store/effects/block.effects';
import { blockRoutes } from './block.routing';

const exportedComponents: Type<any>[] = [
    fromContainers.BlockComponent,
    fromContainers.BlocksComponent,
    fromComponents.LatestBlocksComponent,
    fromComponents.BlockSummaryComponent
];

@NgModule({
    declarations: [...exportedComponents, BlockTransactionsComponent, BlockTransactionsItemComponent],
    imports: [
        RouterModule.forChild(blockRoutes),
        CommonModule,
        SharedModule,
        StoreModule.forFeature(fromStore.blockFeatureKey, fromStore.reducers),
        EffectsModule.forFeature([BlockEffects])
    ],
    exports: [...exportedComponents]
})
export class BlockModule { }

