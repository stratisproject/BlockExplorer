import { NgModule, Type } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import * as fromComponents from './components';
import * as fromContainers from './containers';
import * as fromStore from './store/reducers';
import { SharedModule } from '../../shared/shared.module';
import { StoreModule } from '@ngrx/store';
import { EffectsModule } from '@ngrx/effects';
import { smartContractRoutes } from './smartcontract.routing';
import * as fromEffects from './store/effects';

const exportedComponents: Type<any>[] = [
    fromContainers.SmartContractActionComponent,
    fromContainers.SmartContractsDashboardComponent,
    fromComponents.SmartContractActionDetail,
    fromComponents.SmartContractActionListComponent,
];

@NgModule({
    declarations: [...exportedComponents],
    imports: [
        RouterModule.forChild(smartContractRoutes),
        CommonModule,
        SharedModule,
        StoreModule.forFeature(fromStore.smartContractFeatureKey, fromStore.reducers),
        EffectsModule.forFeature([
            fromEffects.StandardTokensEffects,
            fromEffects.SmartContractActionEffects
        ])
    ],
    exports: [...exportedComponents]
})
export class SmartcontractModule { }
