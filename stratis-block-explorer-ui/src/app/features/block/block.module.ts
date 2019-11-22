import { NgModule, Type } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Route, RouterModule } from '@angular/router';
import * as fromComponents from './components';
import * as fromStore from './store';
import { SharedModule } from '@shared/shared.module';
import { StoreModule } from '@ngrx/store';
import { EffectsModule } from '@ngrx/effects';

export const blockRoutes: Route[] = [
   { path: 'blocks', component: fromComponents.BlocksComponent, data: { breadcrumb: 'Blocks' } },
   {
      path: 'block', data: { breadcrumb: 'Block' }, children: [
         { path: '', redirectTo: '/blocks', pathMatch: "full" },
         { path: ':blockHeight', component: fromComponents.BlockComponent }
      ]
   }
];

const exportedComponents: Type<any>[] = [
   fromComponents.BlockComponent,
   fromComponents.BlocksComponent,
   fromComponents.LatestBlocksComponent
];

@NgModule({
   declarations: [...exportedComponents],
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

