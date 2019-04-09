import { BrowserModule } from '@angular/platform-browser';
import { NgModule, APP_INITIALIZER } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';

import { AppComponent } from './app.component';
import { NxModule } from '@nrwl/nx';
import { RouterModule } from '@angular/router';
import { StoreModule } from '@ngrx/store';
import { EffectsModule } from '@ngrx/effects';
import {
  APP_FEATURE_KEY,
  initialState as appInitialState,
  appReducer
} from './+state/app.reducer';
import { AppEffects } from './+state/app.effects';
import { AppFacade } from './+state/app.facade';
import { StoreDevtoolsModule } from '@ngrx/store-devtools';
import { environment } from '../environments/environment';
import { StoreRouterConnectingModule } from '@ngrx/router-store';
import { storeFreeze } from 'ngrx-store-freeze';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { PrismModule } from '@ngx-prism/core';
import { UiLayoutModule } from '@blockexplorer/ui/layout';
import { FormsModule } from '@angular/forms';
import { uiTransactionsRoutes, UiTransactionsModule, TransactionsPageComponent, uiAddressesRoutes, uiOtherRoutes, uiBlockRoutes } from '@blockexplorer/ui/transactions';
import { StateTransactionsStateModule } from '@blockexplorer/state/transactions-state';
import { SharedModelsModule, AppConfig, APP_CONFIG } from '@blockexplorer/shared/models';
import { StateGlobalStateModule } from '@blockexplorer/state/global-state';
import { ENVIRONMENT, SharedUtilsModule } from '@blockexplorer/shared/utils';
import { UiSmartContractsModule } from '@blockexplorer/ui/smart-contracts';
import { AppConfigService } from './+state/app-config.service';

/**
* Exported function so that it works with AOT
* @param {AppConfigService} configService
* @returns {Function}
*/
export function loadConfigService(configService: AppConfigService): Function 

{
  return () => { 
    return configService.load(); 
  }; 
}

@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserModule,
    NgbModule,
    HttpClientModule,
    FormsModule,
    UiLayoutModule,
    PrismModule,
    UiTransactionsModule,
    UiSmartContractsModule,
    SharedModelsModule,
    SharedUtilsModule.forRoot(),
    StateGlobalStateModule.forRoot(),
    StateTransactionsStateModule.forRoot(),
    NxModule.forRoot(),
    RouterModule.forRoot(
      [
        { path: '', component: TransactionsPageComponent, data: { breadcrumb: 'Home' } },
        { path: 'transactions', children: uiTransactionsRoutes, data: { breadcrumb: 'Transactions' } },
        { path: 'addresses', children: uiAddressesRoutes, data: { breadcrumb: 'Address' } },
        { path: 'blocks', children: uiBlockRoutes, data: { breadcrumb: 'Block' } },
        { path: 'search', children: uiOtherRoutes, data: { breadcrumb: 'Not Found' } }
      ],
      {
        initialNavigation: 'enabled',
        onSameUrlNavigation: 'reload'
      }
    ),
    StoreModule.forRoot(
      { app: appReducer },
      {
        initialState: { app: appInitialState },
        metaReducers: !environment.production ? [storeFreeze] : []
      }
    ),
    EffectsModule.forRoot([AppEffects]),
    !environment.production ? StoreDevtoolsModule.instrument() : [],
    StoreRouterConnectingModule
  ],
  providers: [
    AppConfigService,
    AppFacade,
    { provide: ENVIRONMENT, useValue: environment.production ? 'prod' : 'dev' },
    { provide: APP_INITIALIZER, useFactory: loadConfigService , deps: [AppConfigService], multi: true },
  ],
  bootstrap: [AppComponent]
})
export class AppModule {}
