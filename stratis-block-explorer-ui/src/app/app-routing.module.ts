import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { StoreRouterConnectingModule, routerReducer, RouterState, RouterStateSerializer } from '@ngrx/router-store';

@NgModule({
    imports: [
        RouterModule.forRoot([]),
        StoreRouterConnectingModule.forRoot(),
    ],
    exports: [RouterModule]
})
export class AppRoutingModule { }
