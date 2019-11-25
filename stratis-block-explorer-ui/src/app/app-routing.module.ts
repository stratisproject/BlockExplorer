import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { StoreRouterConnectingModule, routerReducer, RouterState, RouterStateSerializer } from '@ngrx/router-store';
//import { blockRoutes } from './features/block/block.module';
import { dashboardRoutes } from './features/dashboard/dashboard.module';


//const routes: Routes = [
//    ...dashboardRoutes,
//    ...blockRoutes
//];

@NgModule({
    imports: [
        RouterModule.forRoot([]/*routes*/),
        StoreRouterConnectingModule.forRoot()//StoreRouterConnectingModule.forRoot({ stateKey: "router", routerState: RouterState.Minimal }),
    ],
    exports: [RouterModule]
})
export class AppRoutingModule { }
