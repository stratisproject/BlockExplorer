import { NgModule, Type } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Route } from '@angular/router';
import * as fromComponents from './components';
import { SharedModule } from '@shared/shared.module';

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
      CommonModule,
      SharedModule
   ],
   exports: [...exportedComponents]
})
export class BlockModule { }

