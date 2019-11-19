import { NgModule, Type } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Route, RouterModule, provideRoutes } from '@angular/router';
import * as fromBlock from './components';

export const blockRoutes: Route[] = [
   //{
   //   path: 'blocks', component: fromBlock.BlocksComponent, children: [
   //      { path: ':blockHeight', component: fromBlock.BlockComponent }
   //   ]
   //}
   { path: 'blocks', component: fromBlock.BlocksComponent },
   { path: 'block/:blockHeight', component: fromBlock.BlockComponent }
];

const exportedComponents: Type<any>[] = [
   fromBlock.BlockComponent,
   fromBlock.BlocksComponent
];

@NgModule({
   declarations: [...exportedComponents],
   imports: [
      CommonModule
   ],
   exports: [...exportedComponents]
})
export class BlockModule { }

