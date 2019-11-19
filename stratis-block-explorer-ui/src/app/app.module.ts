import { BrowserModule } from '@angular/platform-browser';
import { NgModule, APP_INITIALIZER } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { CoreModule } from './core/core.module';
import { AppConfigService } from './core/services/app-config.service';
import { HttpClientModule } from '@angular/common/http';
import { SharedModule } from './shared/shared.module';
import { MainModule } from '@features/main/main.module';
import { StoreModule } from '@ngrx/store';
import { StoreRouterConnectingModule, routerReducer, RouterState } from '@ngrx/router-store';
import { StoreDevtoolsModule } from '@ngrx/store-devtools';
import { environment } from '../environments/environment';
import { reducers } from './reducers';
import { EffectsModule } from '@ngrx/effects';
import { RouterModule } from '@angular/router';
import { BlockModule, blockRoutes } from './features/block/block.module';

@NgModule({
   declarations: [
      AppComponent
   ],
   imports: [
      BrowserModule,
    //  AppRoutingModule,
      HttpClientModule,
      CoreModule,
      SharedModule,
      MainModule,
      BlockModule,
      StoreModule.forRoot(reducers, {
         runtimeChecks: {
            strictStateImmutability: true,
            strictActionImmutability: true,
            strictStateSerializability: true,
            strictActionSerializability: true,
         }
      }),
      RouterModule.forRoot([
         ...blockRoutes
      ]),
      EffectsModule.forRoot([]),
      StoreRouterConnectingModule.forRoot({ stateKey: "router", routerState: RouterState.Minimal }),
      !environment.production ? StoreDevtoolsModule.instrument() : []
   ],
   providers: [
      AppConfigService,
      { provide: APP_INITIALIZER, useFactory: init_app, deps: [AppConfigService], multi: true }
   ],
   bootstrap: [AppComponent]
})
export class AppModule { }


export function init_app(appLoadService: AppConfigService) {
   return () => appLoadService.load();
}
