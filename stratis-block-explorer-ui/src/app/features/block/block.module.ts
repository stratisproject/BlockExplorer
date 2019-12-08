import { NgModule, Type } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import * as fromComponents from './components';
import * as fromContainers from './containers';
import * as fromStore from './store/reducers';
import { SharedModule } from '@shared/shared.module';
import { StoreModule } from '@ngrx/store';
import { EffectsModule } from '@ngrx/effects';
import * as fromEffects from './store/effects';
import { blockRoutes } from './block.routing';
import { TransactionModule } from '../transaction/transaction.module';

const exportedComponents: Type<any>[] = [
    fromContainers.BlockComponent,
    fromContainers.BlocksComponent,
    fromContainers.LatestBlocksComponent,
    fromComponents.BlockSummaryComponent,
];

@NgModule({
    declarations: [...exportedComponents],
    imports: [
        RouterModule.forChild(blockRoutes),
        CommonModule,
        SharedModule,
        TransactionModule,
        StoreModule.forFeature(fromStore.blockFeatureKey, fromStore.reducers),
        EffectsModule.forFeature([
            fromEffects.BlocksEffects,
            fromEffects.LastBlockEffects
        ])
    ],
    exports: [...exportedComponents]
})
export class BlockModule { }

