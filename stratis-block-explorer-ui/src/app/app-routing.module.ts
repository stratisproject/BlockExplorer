import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { StoreRouterConnectingModule, routerReducer, RouterState } from '@ngrx/router-store';
import { blockRoutes } from './features/block/block.module';


const routes: Routes = [
   ...blockRoutes
];

@NgModule({
   imports: [
      RouterModule.forRoot(routes),
      StoreRouterConnectingModule.forRoot({ stateKey: "router", routerState: RouterState.Minimal }),
   ],
   exports: [RouterModule]
})
export class AppRoutingModule { }
