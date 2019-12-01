import { NgModule, Type } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import * as fromContainers from './containers';
import * as fromStore from './store/reducers';
import { SharedModule } from '../../shared/shared.module';
import { StoreModule } from '@ngrx/store';
import { EffectsModule } from '@ngrx/effects';
import { TransactionEffects } from './store/effects/transaction.effects';
import { transactionRoutes } from './transaction.routing';

const exportedComponents: Type<any>[] = [
    fromContainers.TransactionComponent,
];

@NgModule({
    declarations: [...exportedComponents],
    imports: [
        RouterModule.forChild(transactionRoutes),
        CommonModule,
        SharedModule,
        StoreModule.forFeature(fromStore.transactionFeatureKey, fromStore.reducers),
        EffectsModule.forFeature([TransactionEffects])
    ],
    exports: [...exportedComponents]
})
export class TransactionModule { }
